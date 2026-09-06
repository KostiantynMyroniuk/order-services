using Orders.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddIdentityConfigurations();

builder.AddPersistenceConfigurations();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
