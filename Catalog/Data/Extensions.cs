namespace Catalog.Data;

public static class Extensions
{
    public static void UseMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

        context.Database.Migrate();
        DataSeeder.Seed(context);

    }
}


public class DataSeeder
{
    public static void Seed(ProductDbContext context)
    {
        if (context.Products.Any())
        {
            return;
        }

        context.Products.AddRange(Products);
        context.SaveChanges();
    }

    public static IEnumerable<Product> Products => 
        [
        new Product {Name= "1",Description="1131",ImageUrl="1711",Price=125},
        new Product {Name= "1678",Description="11",ImageUrl="1181",Price=135},
        new Product {Name= "1674",Description="1123",ImageUrl="17611",Price=145},
        new Product {Name= "6781",Description="1165",ImageUrl="11681",Price=155},
        new Product {Name= "13",Description="117",ImageUrl="11177",Price=165},
        new Product {Name= "1345",Description="1154",ImageUrl="111",Price=175}
        ];
}