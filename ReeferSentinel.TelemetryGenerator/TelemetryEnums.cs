namespace ReeferSentinel.TelemetryGenerator
{
    public enum TelemetryType
    {
        TemperatureOnly,
        HumidityOnly,
        Both
    }

    public enum TelemetryScenario
    {
        InRange,
        TemperatureHigh,
        TemperatureLow,
        HumidityHigh,
        HumidityLow,
        BothOutOfRange
    }
}
