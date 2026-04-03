using ReeferSentinel.Monolith.Models;

namespace ReeferSentinel.TelemetryGenerator
{
    public class TelemetryDataGenerator
    {
        private readonly Random _random;

        public TelemetryDataGenerator()
        {
            _random = new Random();
        }

        public List<Telemetry> GenerateTelemetries(
            Container container,
            TelemetryType type,
            TelemetryScenario scenario,
            int count,
            DateTimeOffset startDate,
            int intervalMinutes)
        {
            var telemetries = new List<Telemetry>();
      
            double tempSetpoint = container.TemperatureSetpoint ?? 5.0;
            double humidSetpoint = container.HumiditySetpoint ?? 70.0;

            // Tolerances based on category (default values)
            double tempTolerance = 2.0;
            double humidTolerance = 10.0;

            for (int i = 0; i < count; i++)
            {
                var readingTime = startDate.AddMinutes(i * intervalMinutes);
    
                float temperature = GenerateTemperature(tempSetpoint, tempTolerance, scenario, type);
                float humidity = GenerateHumidity(humidSetpoint, humidTolerance, scenario, type);

                telemetries.Add(new Telemetry
                {
                    ContainerId = container.Id,
                    DateRegistered = readingTime,
                    Temperature = temperature,
                    Humidity = humidity
                });
            }

            return telemetries;
        }

        private float GenerateTemperature(double setpoint, double tolerance, TelemetryScenario scenario, TelemetryType type)
        {
            // If type is humidity only, return value in range
            if (type == TelemetryType.HumidityOnly)
            {
                return (float)(setpoint + (_random.NextDouble() * 2 - 1) * (tolerance * 0.5));
            }

            return scenario switch
            {
                TelemetryScenario.TemperatureHigh => 
                    (float)(setpoint + tolerance + (_random.NextDouble() * 3 + 0.5)),
 
                TelemetryScenario.TemperatureLow => 
                    (float)(setpoint - tolerance - (_random.NextDouble() * 3 + 0.5)),
  
                TelemetryScenario.BothOutOfRange => 
                    _random.NextDouble() > 0.5
                        ? (float)(setpoint + tolerance + (_random.NextDouble() * 3 + 0.5))
                        : (float)(setpoint - tolerance - (_random.NextDouble() * 3 + 0.5)),
         
                _ => // InRange, HumidityHigh, HumidityLow
                    (float)(setpoint + (_random.NextDouble() * 2 - 1) * (tolerance * 0.8))
            };
        }

        private float GenerateHumidity(double setpoint, double tolerance, TelemetryScenario scenario, TelemetryType type)
        {
            // If type is temperature only, return value in range
            if (type == TelemetryType.TemperatureOnly)
            {
                return (float)(setpoint + (_random.NextDouble() * 2 - 1) * (tolerance * 0.5));
            }

            float humidity = scenario switch
            {
                TelemetryScenario.HumidityHigh => 
                    (float)(setpoint + tolerance + (_random.NextDouble() * 15 + 2)),
      
                TelemetryScenario.HumidityLow => 
                    (float)(setpoint - tolerance - (_random.NextDouble() * 15 + 2)),
             
                TelemetryScenario.BothOutOfRange => 
                    _random.NextDouble() > 0.5
                        ? (float)(setpoint + tolerance + (_random.NextDouble() * 15 + 2))
                        : (float)(setpoint - tolerance - (_random.NextDouble() * 15 + 2)),

                _ => // InRange, TemperatureHigh, TemperatureLow
                    (float)(setpoint + (_random.NextDouble() * 2 - 1) * (tolerance * 0.8))
            };

            // Clamp between 0 and 100
            return Math.Max(0, Math.Min(100, humidity));
        }
    }
}
