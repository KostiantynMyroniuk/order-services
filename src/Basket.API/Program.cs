using Basket.API.Apis;
using Basket.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddIdentityConfigurations();

builder.AddServices();

builder.AddPersistenceConfigurations();

builder.AddGrpcServices();

var app = builder.Build();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapBasketApi();

app.Run();
