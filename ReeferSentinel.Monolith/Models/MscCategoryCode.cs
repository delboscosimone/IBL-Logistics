using System.ComponentModel;

namespace ReeferSentinel.Monolith.Models
{
    /// <summary>
    /// Category codes for different types of products transported in refrigerated containers.
    /// Each category has specific temperature and humidity requirements.
    /// </summary>
    public enum MscCategoryCode
    {
        /// <summary>
        /// Pharmaceutical products - MAXIMUM criticality
        /// Temperature: 2-8°C, Very strict control needed
        /// </summary>
        [Description("Pharmaceuticals and Vaccines")]
        Pharmaceuticals = 1,

        /// <summary>
        /// Highly perishable fresh products - HIGH criticality
        /// Includes: fish, meat, fresh dairy
        /// Temperature: -2°C to +4°C
        /// </summary>
        [Description("Highly Perishable Products")]
        HighlyPerishable = 2,

        /// <summary>
        /// Fresh fruits and vegetables - MEDIUM criticality
        /// Temperature: +2°C to +12°C
        /// </summary>
        [Description("Fresh Produce")]
        FreshProduce = 3,

        /// <summary>
        /// Frozen products - MEDIUM-HIGH criticality
        /// Temperature: -18°C to -25°C
        /// </summary>
        [Description("Frozen Products")]
        Frozen = 4,

        /// <summary>
        /// Temperature-controlled products - LOW criticality
        /// Includes: wine, chocolate, coffee
        /// Temperature: +10°C to +25°C
        /// </summary>
        [Description("Temperature Controlled Products")]
        TemperatureControlled = 5
    }

    /// <summary>
    /// Helper methods for MscCategoryCode to get temperature and humidity tolerances
    /// </summary>
    public static class MscCategoryCodeExtensions
    {
        /// <summary>
        /// Gets how much the temperature can vary (in °C) from the setpoint
        /// </summary>
        public static double GetTemperatureTolerance(this MscCategoryCode category)
        {
            return category switch
            {
                MscCategoryCode.Pharmaceuticals => 0.5,           // Very strict
                MscCategoryCode.HighlyPerishable => 1.0,          // Strict
                MscCategoryCode.FreshProduce => 2.0,// Moderate
                MscCategoryCode.Frozen => 3.0, // Moderate
                MscCategoryCode.TemperatureControlled => 5.0,     // Relaxed
                _ => 2.0
            };
        }

        /// <summary>
        /// Gets how much the humidity can vary (in %) from the setpoint
        /// </summary>
        public static double GetHumidityTolerance(this MscCategoryCode category)
        {
            return category switch
            {
                MscCategoryCode.Pharmaceuticals => 5.0,
                MscCategoryCode.HighlyPerishable => 8.0,
                MscCategoryCode.FreshProduce => 10.0,
                MscCategoryCode.Frozen => 15.0,
                MscCategoryCode.TemperatureControlled => 20.0,
                _ => 10.0
            };
        }
    }
}
