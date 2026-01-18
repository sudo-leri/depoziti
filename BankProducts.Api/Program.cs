using System.Globalization;
using BankProducts.Core.Data;
using BankProducts.Core.Services;
using Microsoft.EntityFrameworkCore;

// Set invariant culture for consistent number and date formatting
Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Configure MySQL database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<BankProductsDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 21))));

// Register application services
builder.Services.AddScoped<IDepositService, DepositService>();

// Add API controllers
builder.Services.AddControllers();

// Configure CORS for Vue.js development server
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed the database with sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BankProductsDbContext>();
    await DataSeeder.SeedAsync(context);
}

// Enable CORS
app.UseCors("VueDev");

// Serve static files (Vue.js build output)
app.UseDefaultFiles();
app.UseStaticFiles();

// Map API controllers
app.MapControllers();

// Fallback to index.html for client-side routing (SPA)
app.MapFallbackToFile("index.html");

app.Run();
