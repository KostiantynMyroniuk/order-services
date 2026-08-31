using Identity.API.Apis;
using Identity.API.Extensions;
using Identity.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddPersistence();

builder.AddServices();

builder.AddIdentity();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<UsersDbContext>().Database.MigrateAsync();
}

app.MapIdentityApi();

app.Run();
