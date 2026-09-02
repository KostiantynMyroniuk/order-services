using Basket.API.Apis;
using Basket.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddIdentity();

builder.AddServices();

builder.AddPersistence();

builder.AddGrpcServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapBasketApi();

app.Run();
