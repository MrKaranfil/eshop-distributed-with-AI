
namespace Catalog.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

public class ProductAIService(ProductDbContext dbContext, IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IVectorStoreRecordCollection<int, ProductVector> productVectorCollection)
{
    public async Task<string> SupportAsync(string query)
    {
        var systemPrompt = """
             Merhaba yeğenim 
            
            """;

        var chatHistory = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System,systemPrompt),
            new ChatMessage(ChatRole.User,query)
        };

        var resultPrompt = await chatClient.GetResponseAsync(chatHistory);
        return resultPrompt.Message.Contents[0].ToString();
    }
    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        if (!await productVectorCollection.CollectionExistsAsync())
        {
            await InitEmbeddingAsync();
        } 
        //
        var queryEmbedding = await embeddingGenerator.GenerateEmbeddingVectorAsync(query);

        var vectorSearchOptions = new VectorSearchOptions
        {
            Top = 1,
            VectorPropertyName = "Vector"
        };

        var results
            = await productVectorCollection.VectorizedSearchAsync(queryEmbedding, vectorSearchOptions);

        List<Product> products = new List<Product>();

        await foreach (var item in results.Results)
        {
            products.Add(new Product
            {
                Id = item.Record.Id,
                Name = item.Record.Name,
                Description = item.Record.Description,
                ImageUrl = item.Record.ImageUrl,
                Price = item.Record.Price,
            });
        }

        return products;
    }

    private async Task InitEmbeddingAsync()
    {
        await productVectorCollection.CreateCollectionIfNotExistsAsync();

        var products = await dbContext.Products.ToListAsync();

        foreach (var product in products)
        {
            var productInfo = $"{product.Name} is a product that costs [{product.Price}] and ";

            var productVector = new ProductVector
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Vector = await embeddingGenerator.GenerateEmbeddingVectorAsync(productInfo)
            };

            await productVectorCollection.UpsertAsync(productVector);
        }
    }
}
