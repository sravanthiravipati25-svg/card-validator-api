using Microsoft.EntityFrameworkCore;
using Validation.CardService.Data;
using Validation.CardService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("CardValidationDb")
    ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

builder.Services.AddDbContext<CardValidationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Trie is built once at startup and only read from concurrently — safe as a singleton.
builder.Services.AddSingleton<ILuhnValidator, LuhnValidator>();
builder.Services.AddSingleton<IBinLookupService, BinTrie>();

// EF Core DbContext is scoped, so anything depending on it must be scoped too.
builder.Services.AddScoped<ICardValidationRepository, CardValidationRepository>();
builder.Services.AddScoped<ICardValidationService, CardValidationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactDev");

app.UseAuthorization();

app.MapControllers();

app.Run();