using Infrastructure;
using MeShopV1.Cart;
using MeShopV1.Catalog;
using MeShopV1.Inventory;
using MeShopV1.Orders;
using MeShopV1.Payments;
using MeShopV1.Shipments;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenApi();
builder.AddDatabase();
builder.AddAuth();
builder.AddBackgroundJobs();

builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ShipmentService>();

builder.Services.AddScoped<PaymentClient>();
builder.Services.AddScoped<CarrierClient>();

var app = builder.Build();

app.UseAuth();
app.UseOpenApi();

CartApi.Map(app);
CatalogApi.Map(app);
InventoryApi.Map(app);
OrderApi.Map(app);

app.MigrateDatabase();

app.Run();
