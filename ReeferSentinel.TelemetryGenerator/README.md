# Reefer Sentinel - Automatic Telemetry Generator

## Description

This console project allows you to automatically generate telemetries (temperature and humidity) for containers in the ReeferSentinel database.

## How to use

### 1. Run the project

```bash
cd ReeferSentinel.TelemetryGenerator
dotnet run
```

Or from Visual Studio: set `ReeferSentinel.TelemetryGenerator` as startup project and press F5.

### 2. Follow the interactive instructions

The program will guide you through these steps:

1. **Container Selection**: Displays all available containers and asks for the ID
2. **Telemetry Type**:
   - `1` - Temperature Only
 - `2` - Humidity Only
   - `3` - Both

3. **Scenario**:

   For Temperature Only:
   - `1` - Values IN RANGE (compliant with setpoint +/- tolerance)
   - `2` - Temperature HIGH (above setpoint + tolerance)
   - `3` - Temperature LOW (below setpoint - tolerance)
   
   For Humidity Only:
   - `1` - Values IN RANGE (compliant with setpoint +/- tolerance)
   - `2` - Humidity HIGH (above setpoint + tolerance)
   - `3` - Humidity LOW (below setpoint - tolerance)
   
   For Both:
   - `1` - Values IN RANGE (compliant with setpoint +/- tolerance)
   - `2` - Temperature HIGH (above setpoint + tolerance)
   - `3` - Temperature LOW (below setpoint - tolerance)
   - `4` - Humidity HIGH (above setpoint + tolerance)
   - `5` - Humidity LOW (below setpoint - tolerance)
   - `6` - BOTH OUT OF RANGE

4. **Number of readings**: How many telemetries to generate (default: 10)

5. **Start date/time**: Date/time of the first reading (default: now)

6. **Interval**: Minutes between readings (default: 60)

### 3. Preview and confirmation

The program will show a preview of the first 10 generated telemetries and ask for confirmation before saving to the database.

## Configuration

### Connection String

Modify the connection string in the `Program.cs` file (line ~11) if necessary:

```csharp
var connectionString = "Server=.\\SQLEXPRESS;Database=ReeferSentinelDb;Trusted_Connection=True;TrustServerCertificate=True;";
```

## Example output

```
========================================================
  REEFER SENTINEL - Automatic Telemetry Generator
========================================================

[OK] Database connection successful!

AVAILABLE CONTAINERS:
-----------------------------------------------------
ID: 1   | Temp Setpoint: 5.0C | Humidity: 60%
  ID: 2| Temp Setpoint: 1.0C | Humidity: 85%

> Enter container ID: 1

[OK] Container selected: ID 1
   Temperature Setpoint: 5.0C
   Humidity Setpoint: 60.0%

TELEMETRY TYPE TO GENERATE:
  1. Temperature Only
  2. Humidity Only
  3. Both (Temperature + Humidity)

> Choose (1-3): 3

TELEMETRY SCENARIO:
  1. Values IN RANGE (compliant with setpoint +/- tolerance)
  2. Temperature HIGH (above setpoint + tolerance)
  ...

> Choose (1-6): 2

> How many telemetry readings to generate? (default: 10): 20

> Start date/time (leave empty for NOW): 

> Interval between readings in MINUTES (default: 60): 30

[PROCESSING] Generating telemetries...

===========================================================================
   GENERATED TELEMETRIES PREVIEW
===========================================================================

Date/Time   |   Temperature |   Humidity | Status
---------------------------------------------------------------------------
2026-01-03 15:30:00 |       8.23C |61.5% | [ALERT] [OK]
2026-01-03 16:00:00 |       7.89C |     59.8% | [ALERT] [OK]
...

SUMMARY:
   - Total readings: 20
   - Container ID: 1
   - Period: from 2026-01-03 15:30 to 2026-01-03 05:30

? Do you want to save these telemetries to the database? (y/n): y

[OK] 20 telemetries saved successfully to database!

Program terminated. Press any key to exit...
```

## Use cases

### Scenario 1: Test out-of-range temperatures
- Generate telemetries with high temperature to test alerts

### Scenario 2: Historical data population
- Create historical datasets with realistic values for dashboards and analytics

### Scenario 3: Anomaly simulation
- Test system behavior with out-of-range values

## Troubleshooting

### Database connection error
- Verify that SQL Server is running
- Check that the `ReeferSentinelDb` database exists
- Verify the connection string

### No containers found
- Make sure you have run the migrations
- Verify that there are containers in the database

