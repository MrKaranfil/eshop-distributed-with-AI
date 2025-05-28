

var builder = WebApplication.CreateBuilder(args);



builder.AddServiceDefaults();
builder.AddRedisDistributedCache(connectionName: "cache");
builder.Services.AddScoped<BasketService>();
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
    serviceName:"keycloak",
    realm:"eshop",
    configureOptions: options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = "account";
    });
builder.Services.AddAuthorization();


builder.Services.AddHttpClient<CatalogApiClient>(client =>
{
    client.BaseAddress = new("https+http://catalog");
});
// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapBasketEndpoints();

app.UseAuthentication();

app.UseAuthorization();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.Run();