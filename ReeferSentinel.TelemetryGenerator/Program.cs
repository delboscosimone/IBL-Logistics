using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ReeferSentinel.Monolith.Data;
using ReeferSentinel.Monolith.Models;
using ReeferSentinel.TelemetryGenerator;

Console.WriteLine("========================================================");
Console.WriteLine("  REEFER SENTINEL - Automatic Telemetry Generator");
Console.WriteLine("========================================================\n");

// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get connection string from configuration
var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("[ERROR] Connection string not found in appsettings.json!");
    return;
}

// Setup DbContext
var optionsBuilder = new DbContextOptionsBuilder<AppDatabase>();
optionsBuilder.UseSqlServer(connectionString);

using var dbContext = new AppDatabase(optionsBuilder.Options);

// Verify connection
try
{
    await dbContext.Database.CanConnectAsync();
    Console.WriteLine("[OK] Database connection successful!\n");
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] Database connection failed: {ex.Message}");
    return;
}

// Show available containers
var containers = await dbContext.Containers.ToListAsync();
if (!containers.Any())
{
    Console.WriteLine("[ERROR] No containers found in database!");
    return;
}

Console.WriteLine("AVAILABLE CONTAINERS:");
Console.WriteLine("-----------------------------------------------------");
foreach (var container in containers)
{
    Console.WriteLine($"  ID: {container.Id,-3} | Temp Setpoint: {container.TemperatureSetpoint:F1}C | Humidity: {container.HumiditySetpoint:F0}%");
}
Console.WriteLine();

// Input Container ID
Console.Write("> Enter container ID: ");
var containerIdInput = Console.ReadLine();

if (!int.TryParse(containerIdInput, out int containerId))
{
    Console.WriteLine("[ERROR] Invalid ID!");
    return;
}

var selectedContainer = containers.FirstOrDefault(c => c.Id == containerId);
if (selectedContainer == null)
{
    Console.WriteLine($"[ERROR] Container with ID {containerId} not found!");
    return;
}

Console.WriteLine($"\n[OK] Container selected: ID {selectedContainer.Id}");
Console.WriteLine($"   Temperature Setpoint: {selectedContainer.TemperatureSetpoint:F1}C");
Console.WriteLine($"   Humidity Setpoint: {selectedContainer.HumiditySetpoint:F0}%\n");

// Telemetry type
Console.WriteLine("TELEMETRY TYPE TO GENERATE:");
Console.WriteLine("  1. Temperature Only");
Console.WriteLine("  2. Humidity Only");
Console.WriteLine("  3. Both (Temperature + Humidity)");
Console.Write("\n> Choose (1-3): ");
var typeChoice = Console.ReadLine();

TelemetryType telemetryType = typeChoice switch
{
    "1" => TelemetryType.TemperatureOnly,
    "2" => TelemetryType.HumidityOnly,
    "3" => TelemetryType.Both,
    _ => TelemetryType.Both
};

// Scenario selection
TelemetryScenario scenario;

if (telemetryType == TelemetryType.TemperatureOnly)
{
    Console.WriteLine("\nTEMPERATURE SCENARIO:");
    Console.WriteLine("  1. IN RANGE (compliant with setpoint +/- tolerance)");
    Console.WriteLine("  2. HIGH (above setpoint + tolerance)");
    Console.WriteLine("  3. LOW (below setpoint - tolerance)");
    Console.Write("\n> Choose (1-3): ");
    var scenarioChoice = Console.ReadLine();

    scenario = scenarioChoice switch
    {
        "1" => TelemetryScenario.InRange,
        "2" => TelemetryScenario.TemperatureHigh,
        "3" => TelemetryScenario.TemperatureLow,
        _ => TelemetryScenario.InRange
    };
}
else if (telemetryType == TelemetryType.HumidityOnly)
{
    Console.WriteLine("\nHUMIDITY SCENARIO:");
    Console.WriteLine("  1. IN RANGE (compliant with setpoint +/- tolerance)");
    Console.WriteLine("  2. HIGH (above setpoint + tolerance)");
    Console.WriteLine("  3. LOW (below setpoint - tolerance)");
    Console.Write("\n> Choose (1-3): ");
    var scenarioChoice = Console.ReadLine();

    scenario = scenarioChoice switch
    {
        "1" => TelemetryScenario.InRange,
        "2" => TelemetryScenario.HumidityHigh,
        "3" => TelemetryScenario.HumidityLow,
        _ => TelemetryScenario.InRange
    };
}
else // Both
{
    // Step 1: Temperature scenario
    Console.WriteLine("\nTEMPERATURE SCENARIO:");
    Console.WriteLine("  1. IN RANGE (compliant with setpoint +/- tolerance)");
    Console.WriteLine("  2. HIGH (above setpoint + tolerance)");
    Console.WriteLine("  3. LOW (below setpoint - tolerance)");
    Console.Write("\n> Choose (1-3): ");
    var tempScenarioChoice = Console.ReadLine();

    var tempScenario = tempScenarioChoice switch
    {
        "1" => TelemetryScenario.InRange,
        "2" => TelemetryScenario.TemperatureHigh,
        "3" => TelemetryScenario.TemperatureLow,
        _ => TelemetryScenario.InRange
    };

    // Step 2: Humidity scenario
    Console.WriteLine("\nHUMIDITY SCENARIO:");
    Console.WriteLine("  1. IN RANGE (compliant with setpoint +/- tolerance)");
    Console.WriteLine("  2. HIGH (above setpoint + tolerance)");
    Console.WriteLine("  3. LOW (below setpoint - tolerance)");
    Console.Write("\n> Choose (1-3): ");
    var humidScenarioChoice = Console.ReadLine();

    var humidScenario = humidScenarioChoice switch
    {
        "1" => TelemetryScenario.InRange,
        "2" => TelemetryScenario.HumidityHigh,
        "3" => TelemetryScenario.HumidityLow,
        _ => TelemetryScenario.InRange
    };

    // Combine scenarios
    if (tempScenario != TelemetryScenario.InRange && humidScenario != TelemetryScenario.InRange)
    {
        scenario = TelemetryScenario.BothOutOfRange;
    }
    else if (tempScenario != TelemetryScenario.InRange)
    {
        scenario = tempScenario;
    }
    else if (humidScenario != TelemetryScenario.InRange)
    {
        scenario = humidScenario;
    }
    else
    {
        scenario = TelemetryScenario.InRange;
    }
}

// Number of readings
Console.Write("\n> How many telemetry readings to generate? (default: 10): ");
var countInput = Console.ReadLine();
int count = int.TryParse(countInput, out int parsedCount) && parsedCount > 0 ? parsedCount : 10;

// Start date/time
Console.Write("> Start date/time (leave empty for NOW): ");
var dateInput = Console.ReadLine();
DateTimeOffset startDate = string.IsNullOrWhiteSpace(dateInput)
    ? DateTimeOffset.Now
    : DateTimeOffset.TryParse(dateInput, out var parsed) ? parsed : DateTimeOffset.Now;

// Interval between readings (in minutes)
Console.Write("> Interval between readings in MINUTES (default: 60): ");
var intervalInput = Console.ReadLine();
int intervalMinutes = int.TryParse(intervalInput, out int parsedInterval) && parsedInterval > 0 ? parsedInterval : 60;

// Generate telemetries
Console.WriteLine("\n[PROCESSING] Generating telemetries...\n");

var generator = new TelemetryDataGenerator();
var generatedTelemetries = generator.GenerateTelemetries(
    selectedContainer,
    telemetryType,
    scenario,
    count,
    startDate,
    intervalMinutes
);

// Show preview
Console.WriteLine("===========================================================================");
Console.WriteLine("   GENERATED TELEMETRIES PREVIEW");
Console.WriteLine("===========================================================================\n");

Console.WriteLine($"{"Date/Time",-22} | {"Temperature",12} | {"Humidity",10} | {"Status"}");
Console.WriteLine(new string('-', 75));

foreach (var telemetry in generatedTelemetries.Take(Math.Min(10, generatedTelemetries.Count)))
{
    var tempStatus = GetTemperatureStatus(telemetry.Temperature ?? 0, selectedContainer.TemperatureSetpoint ?? 0);
    var humidStatus = GetHumidityStatus(telemetry.Humidity ?? 0, selectedContainer.HumiditySetpoint ?? 0);

    Console.WriteLine($"{telemetry.DateRegistered:yyyy-MM-dd HH:mm:ss} | {telemetry.Temperature,10:F2}C | {telemetry.Humidity,8:F1}% | {tempStatus} {humidStatus}");
}

if (generatedTelemetries.Count > 10)
{
    Console.WriteLine($"... ({generatedTelemetries.Count - 10} more readings)");
}

Console.WriteLine($"\nSUMMARY:");
Console.WriteLine($"   - Total readings: {generatedTelemetries.Count}");
Console.WriteLine($"   - Container ID: {selectedContainer.Id}");
Console.WriteLine($"   - Period: from {generatedTelemetries.First().DateRegistered:yyyy-MM-dd HH:mm} to {generatedTelemetries.Last().DateRegistered:yyyy-MM-dd HH:mm}");

 // Confirm save
Console.Write("\n? Do you want to save these telemetries to the database? (y/n): ");
var confirm = Console.ReadLine()?.Trim().ToLower();

if (confirm == "y" || confirm == "yes")
{
    try
    {
        await dbContext.Telemetries.AddRangeAsync(generatedTelemetries);
        await dbContext.SaveChangesAsync();

        Console.WriteLine($"\n[OK] {generatedTelemetries.Count} telemetries saved successfully to database!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[ERROR] Error during save: {ex.Message}");
    }
}
else
{
    Console.WriteLine("\n[CANCELLED] Operation cancelled. No telemetries saved.");
}

Console.WriteLine("\nProgram terminated. Press any key to exit...");
Console.ReadKey();

// Helper methods
static string GetTemperatureStatus(float temp, double setpoint)
{
    var diff = Math.Abs(temp - setpoint);
    return diff <= 2 ? "[OK]" : diff <= 5 ? "[WARN]" : "[ALERT]";
}

static string GetHumidityStatus(float humidity, double setpoint)
{
    var diff = Math.Abs(humidity - setpoint);
    return diff <= 10 ? "[OK]" : diff <= 20 ? "[WARN]" : "[ALERT]";
}
