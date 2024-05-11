using AuthBugDemo.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure host.
builder.ConfigureHost();

var configuration = builder.Configuration;

// Configure services.
builder.Services.ConfigureServices(configuration);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
