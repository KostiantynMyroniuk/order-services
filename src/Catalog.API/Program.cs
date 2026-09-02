using Catalog.API.Extensions;
using Catalog.API.Grpc;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultAuthentication();
builder.AddPersistence();
builder.AddGrpcServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.MigrateDatabase();
    await app.SeedProducts();
}

app.MapGrpcService<CatalogGrpcService>();

app.Run();
