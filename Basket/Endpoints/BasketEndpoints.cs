
namespace Basket.Endpoints;

public static class BasketEndpoints
{
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("basket");

        group.MapGet("/{username}", async (string userName, BasketService service) =>
        {
            var shoppingCart = await service.GetBasket(userName);

            if (shoppingCart is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(shoppingCart);
        })
            .WithName("GetBasket")
            .Produces<ShoppingCart>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();


        group.MapPost("/", async (ShoppingCart s, BasketService service) =>
        {
            await service.UpdateBasket(s);
            return Results.Created("UpdateBasket", s);

        })
            .WithName("UpdateBasket")
            .Produces<ShoppingCart>(StatusCodes.Status201Created)
            .RequireAuthorization();

        group.MapDelete("/{username}", async (string userName, BasketService service) =>
        {
            await service.DeleteBasket(userName);
            return Results.NoContent();
        })
            .WithName("DeleteBasket")
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization();
    }
}
