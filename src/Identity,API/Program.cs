using Identity_API.Apis;
using Identity_API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddPersistence();

builder.AddIdentity();

var app = builder.Build();

app.MapIdentityApi();

app.Run();
