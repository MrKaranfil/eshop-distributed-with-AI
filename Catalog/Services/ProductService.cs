using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Catalog.Services
{
    public class ProductService(ProductDbContext productDbContext,IBus bus)
    {

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await productDbContext.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await productDbContext.Products.FindAsync(id);
        }
        public async Task CreateProductAsync(Product product)
        {
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product updatedProduct, Product inputProduct)
        {

            if (updatedProduct.Price != inputProduct.Price)
            {
                var integrationEvent = new ProductPriceChangedIntegrationEvent
                {
                    ProductId = updatedProduct.Id,
                    Price = inputProduct.Price,
                    Description = updatedProduct.Description,
                    ImageUrl = updatedProduct.ImageUrl,
                    Name = updatedProduct.Name  
                    
                };
                await bus.Publish(integrationEvent);
            }

            // BURADA İKİLİ YAZMA PROBLEMİ OLUŞABİLİR OUTBOX PATTERN VEYA SAGA KULLANILIRSA ÇÖZÜLEBİLİR ŞUANLIK YOK 
            updatedProduct.Name = inputProduct.Name;
            updatedProduct.Description = inputProduct.Description;
            updatedProduct.Price = inputProduct.Price;
            updatedProduct.ImageUrl = inputProduct.ImageUrl;

            productDbContext.Products.Update(updatedProduct);
            await productDbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product deletedProduct)
        {
            productDbContext.Products.Remove(deletedProduct);
            await productDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductAsync(string query)
        {
            return await productDbContext.Products
                .Where(x => x.Name.Contains(query))
                .ToListAsync(); 
        }
    }
}
