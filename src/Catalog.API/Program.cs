using Catalog.API.Extensions;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultAuthentication();
builder.AddPersistence();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.MigrateDatabase();
    await app.SeedProducts();
}

app.Run();
