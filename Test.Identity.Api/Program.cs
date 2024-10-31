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
            builder.WithOrigins("http://localhost:5173", "https://localhost:5001") // Allow multiple origins here
                   .AllowAnyHeader()           // Allow all headers (Authorization, Content-Type, etc.)
                   .AllowAnyMethod()           // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
                   .AllowCredentials();        // Allow credentials like cookies, authorization headers, etc.
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
    var audiences = new List<string>
    {
        "http://localhost:5173",       // React frontend
        "https://localhost:5001"       // DBZ API or other API endpoints
    };

    return new
    {
        access_token = tokenGenerator.GenerateToken(request.Email, audiences)
    };
});

app.Run();