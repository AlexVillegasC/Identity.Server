using Microsoft.AspNetCore.Identity.Data;
using Test.Identity.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<TokenGenerator>();

// Update CORS to allow your specific frontend origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")  // Allow your frontend React app's origin
                   .AllowAnyHeader()  // Allow all headers (Authorization, Content-Type, etc.)
                   .AllowAnyMethod()  // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
                   .AllowCredentials();  // Allow credentials like cookies, authorization headers, etc.
        });
});

var app = builder.Build();

// Enable CORS globally for the frontend app
app.UseCors("AllowFrontendApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Login endpoint
app.MapPost("/login", (LoginRequest request, TokenGenerator tokenGenerator) =>
{
    return new
    {
        access_token = tokenGenerator.GenerateToken(request.Email)
    };
});

app.Run();