namespace ReeferSentinel.Monolith.Models
{
    public class Product
    {
        public int Id { get; set; }

        // 1-n relationship
        public int ContainerId { get; set; } //fk
        public Container Container { get; set; }
        public MscCategoryCode CategoryCode { get; set; }
        public string ProductName { get; set; }
        public string Notes { get; set; }
        public double Weight { get; set; } //kg
        public double Volume { get; set; } //m^3

        /// <summary>
        /// Boolean for soft delete
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
