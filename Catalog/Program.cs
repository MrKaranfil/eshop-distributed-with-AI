


using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();
// Add services to the container.




builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAIService>();
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());


// register ollama-based chat & embedding
builder.AddOllamaSharpChatClient("ollama-llama3");
builder.AddOllamaSharpEmbeddingGenerator("ollama-all-minilm");

// register an in memory vector store
builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");

var app = builder.Build();


app.MapDefaultEndpoints();



app.UseMigration();

app.MapProductEndpoints();



// Configure the HTTP request pipeline.
app.UseHttpsRedirection();


app.Run();
