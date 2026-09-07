using Microsoft.EntityFrameworkCore;
using Orders.API.Apis;
using Orders.API.Extensions;
using Orders.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();

builder.AddIdentityConfigurations();

builder.AddGrpcServices();

builder.AddPersistenceConfigurations();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<OrdersDbContext>().Database.MigrateAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapOrdersApi();

app.Run();
