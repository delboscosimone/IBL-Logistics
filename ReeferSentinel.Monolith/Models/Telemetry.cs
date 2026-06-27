namespace ReeferSentinel.Monolith.Models
{
    /// <summary>
    /// Represents a temperature and humidity reading from a container sensor.
    /// Each container has multiple telemetry readings over time.
    /// </summary>
    public class Telemetry
    {
        /// <summary>
        /// Unique identifier for this telemetry reading
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the container this reading belongs to
        /// </summary>
        public int ContainerId { get; set; }

        /// <summary>
        /// Reference to the container this reading belongs to
        /// </summary>
        public Container Container { get; set; }

        /// <summary>
        /// When this reading was recorded
        /// </summary>
        public DateTime DateRegistered { get; set; }



        /// <summary>
        /// Temperature reading in �C
        /// </summary>
        public float? Temperature { get; set; }

        /// <summary>
        /// Humidity reading in %
        /// </summary>
        public float? Humidity { get; set; }
    }
}
