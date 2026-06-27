using Microsoft.EntityFrameworkCore;
using ReeferSentinel.Monolith.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7061",
                "http://localhost:5061",
                "https://localhost:7268",
                "http://localhost:5268")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDatabase>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<AppDatabase>();
    var autoRepair = builder.Configuration.GetValue("Database:AutoRepairOnStartup", true);
    try
    {
        database.Database.EnsureCreated();
        _ = database.Ports.Any();
    }
    catch when (autoRepair)
    {
        database.Database.EnsureDeleted();
        database.Database.EnsureCreated();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("BlazorClient");
app.UseAuthorization();
app.MapControllers();

app.Run();
