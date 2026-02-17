using Microsoft.EntityFrameworkCore;
using ReeferSentinel.Monolith.Data;

var builder = WebApplication.CreateBuilder(args);

// Add API controllers
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database connection
builder.Services.AddDbContext<AppDatabase>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Enable Swagger in development mode 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect root URL to Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
