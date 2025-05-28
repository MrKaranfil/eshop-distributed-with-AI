namespace Catalog.Endpoints;

public static class ProductEndpoint
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products");

        // GET All
        group.MapGet("/", async (ProductService service) =>
        {
            var products = await service.GetProductsAsync();
            if (products is null) return Results.NotFound();

            return Results.Ok(products);

        })
            .WithName("GetAllProducts")
            .Produces<List<Product>>(StatusCodes.Status200OK);


        group.MapGet("/{id}", async (int id, ProductService service) =>
        {
            var products = await service.GetProductByIdAsync(id);
            if (products is null) return Results.NotFound();

            return Results.Ok(products);

        })
          .WithName("GetProductById")
          .Produces<Product>(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status404NotFound);


        group.MapPost("/", async (Product product, ProductService service) =>
        {
            await service.CreateProductAsync(product);

            return Results.Created($"/products/{product.Id}", product);

        })
          .WithName("CreateProduct")
          .Produces<Product>(StatusCodes.Status201Created);

        group.MapPut("/{id}", async (int id, Product inputProduct, ProductService service) =>
        {
            var updatedProduct = await service.GetProductByIdAsync(id);
            if (updatedProduct is null) return Results.NotFound();

            await service.UpdateProductAsync(updatedProduct, inputProduct);
            return Results.NoContent();

        })
        .WithName("UpdateProduct")
        .Produces<Product>(StatusCodes.Status404NotFound)
        .Produces<Product>(StatusCodes.Status204NoContent);


        group.MapDelete("/{id}", async (int id, ProductService service) =>
        {
            var updatedProduct = await service.GetProductByIdAsync(id);
            if (updatedProduct is null) return Results.NotFound();

            await service.DeleteProductAsync(updatedProduct);
            return Results.NoContent();

        })
      .WithName("DeleteProduct")
      .Produces<Product>(StatusCodes.Status404NotFound)
      .Produces<Product>(StatusCodes.Status204NoContent);


        group.MapGet("/support/{query}", async (string query, ProductAIService service) =>
        {
            var res = await service.SupportAsync(query);
            return Results.Ok(res);
        })
         .WithName("Support")
         .Produces(StatusCodes.Status200OK);

        group.MapGet("search/{query}", async (string query, ProductService service) =>
        {
            var products = await service.SearchProductAsync(query);

            return Results.Ok(products);
        })
   .WithName("SearchProducts")
   .Produces<List<Product>>(StatusCodes.Status200OK);

        group.MapGet("/aisearch/{query}", async (string query, ProductAIService service) =>
        {
            var res = await service.SearchProductsAsync(query);
            return Results.Ok(res);
        })
        .WithName("AISearchProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);
    }

}
