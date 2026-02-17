using Microsoft.EntityFrameworkCore;
using ReeferSentinel.Monolith.Models;

namespace ReeferSentinel.Monolith.Data
{
    /// <summary>
    /// Static class responsible for seeding the database with sample data.
    /// Contains methods to create test containers and telemetry readings.
    /// </summary>
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Seeds the database with sample containers and telemetry data for testing
        /// </summary>
        public static void SeedDatabase(ModelBuilder modelBuilder)
        {
            var baseTelemetryDate = new DateTimeOffset(2026, 02, 01, 0, 0, 0, TimeSpan.Zero);
            var baseShippingDate = new DateTime(2026, 02, 10);

            var bookings = new List<Booking>();
            var containers = new List<Container>();
            var products = new List<Product>();
            var telemetries = new List<Telemetry>();

            var random = new Random(42);

            int bookingId = 1;
            int containerId = 1;
            int productId = 1;
            int telemetryId = 1;

            // -------------------------
            // BOOKING 1 (Agency 2001)
            // -------------------------
            var booking1 = CreateBooking(
                id: bookingId++,
                bkNumber: "BK-000001",
                agencyCode: "MSC-AG-2001",
                shippingDate: baseShippingDate.AddDays(0),
                customerName: "Mario",
                customerSurname: "Rossi",
                customerTaxCode: "RSSMRA80A01H501U",
                customerCompany: "Pharma Italia S.p.A.",
                consigneeName: "Luigi",
                consigneeSurname: "Bianchi",
                consigneeTaxCode: "BNCLGU82B02H501K",
                consigneeCompany: "Ospedali Riuniti S.r.l.",
                originNation: "IT",
                originCity: "Torino",
                originAddress: "Via Roma 1",
                originPostalCode: "10100",
                destinationNation: "DE",
                destinationCity: "Hamburg",
                destinationAddress: "Hafenstrasse 10",
                destinationPostalCode: "20095"
            );
            bookings.Add(booking1);

            // Container 1: Pharmaceuticals with HIGH temperature
            var c1 = CreateContainer(
                id: containerId++,
                containerNumber: "MSCU1000",
                category: MscCategoryCode.Pharmaceuticals,
                agentCode: "AG001",
                bookingId: booking1.Id
            );
            containers.Add(c1);
            products.AddRange(CreateProductsForContainer(ref productId, c1, random, itemCount: 6));
            UpdateContainerTotalsFromProducts(c1, products);
            telemetries.AddRange(CreateTelemetryReadings(
                ref telemetryId, c1, baseTelemetryDate,
                isTemperatureTooHigh: true, isTemperatureTooLow: false, isHumidityTooHigh: false,
                random
            ));

            // Container 2: Highly Perishable with HIGH temperature
            var c2 = CreateContainer(
                id: containerId++,
                containerNumber: "MSCU1001",
                category: MscCategoryCode.HighlyPerishable,
                agentCode: "AG001",
                bookingId: booking1.Id
            );
            containers.Add(c2);
            products.AddRange(CreateProductsForContainer(ref productId, c2, random, itemCount: 6));
            UpdateContainerTotalsFromProducts(c2, products);
            telemetries.AddRange(CreateTelemetryReadings(
                ref telemetryId, c2, baseTelemetryDate,
                isTemperatureTooHigh: true, isTemperatureTooLow: false, isHumidityTooHigh: false,
                random
            ));

            // -------------------------
            // BOOKING 2 (Agency 2002)
            // -------------------------
            var booking2 = CreateBooking(
                id: bookingId++,
                bkNumber: "BK-000002",
                agencyCode: "MSC-AG-2002",
                shippingDate: baseShippingDate.AddDays(1),
                customerName: "Giulia",
                customerSurname: "Verdi",
                customerTaxCode: "VRDGLI90C41F205X",
                customerCompany: "Ortofrutta Nord S.r.l.",
                consigneeName: "Anna",
                consigneeSurname: "Schmidt",
                consigneeTaxCode: "SCHNNA91D55Z112A",
                consigneeCompany: "Fresh Markets GmbH",
                originNation: "IT",
                originCity: "Cuneo",
                originAddress: "Strada Provinciale 20",
                originPostalCode: "12100",
                destinationNation: "DE",
                destinationCity: "Berlin",
                destinationAddress: "Marktweg 7",
                destinationPostalCode: "10115"
            );
            bookings.Add(booking2);

            // Container 3: Fresh Produce with LOW temperature
            var c3 = CreateContainer(
                id: containerId++,
                containerNumber: "MSCU2000",
                category: MscCategoryCode.FreshProduce,
                agentCode: "AG002",
                bookingId: booking2.Id
            );
            containers.Add(c3);
            products.AddRange(CreateProductsForContainer(ref productId, c3, random, itemCount: 7));
            UpdateContainerTotalsFromProducts(c3, products);
            telemetries.AddRange(CreateTelemetryReadings(
                ref telemetryId, c3, baseTelemetryDate,
                isTemperatureTooHigh: false, isTemperatureTooLow: true, isHumidityTooHigh: false,
                random
            ));

            // Container 4: Frozen with LOW temperature
            var c4 = CreateContainer(
                id: containerId++,
                containerNumber: "MSCU2001",
                category: MscCategoryCode.Frozen,
                agentCode: "AG002",
                bookingId: booking2.Id
            );
            containers.Add(c4);
            products.AddRange(CreateProductsForContainer(ref productId, c4, random, itemCount: 6));
            UpdateContainerTotalsFromProducts(c4, products);
            telemetries.AddRange(CreateTelemetryReadings(
                ref telemetryId, c4, baseTelemetryDate,
                isTemperatureTooHigh: false, isTemperatureTooLow: true, isHumidityTooHigh: false,
                random
            ));

            // -------------------------
            // BOOKING 3 (Agency 2003)
            // -------------------------
            var booking3 = CreateBooking(
                id: bookingId++,
                bkNumber: "BK-000003",
                agencyCode: "MSC-AG-2003",
                shippingDate: baseShippingDate.AddDays(2),
                customerName: "Paolo",
                customerSurname: "Neri",
                customerTaxCode: "NRIPLA85E10L219D",
                customerCompany: "Export Food Italia S.p.A.",
                consigneeName: "Sophie",
                consigneeSurname: "Dubois",
                consigneeTaxCode: "DBSSPH88F01Z110B",
                consigneeCompany: "Distribution FR SAS",
                originNation: "IT",
                originCity: "Genova",
                originAddress: "Via del Porto 22",
                originPostalCode: "16100",
                destinationNation: "FR",
                destinationCity: "Lyon",
                destinationAddress: "Rue du Froid 3",
                destinationPostalCode: "69000"
            );
            bookings.Add(booking3);

            // Container 5: Fresh Produce with HIGH humidity
            var c5 = CreateContainer(
                id: containerId++,
                containerNumber: "MSCU3000",
                category: MscCategoryCode.FreshProduce,
                agentCode: "AG003",
                bookingId: booking3.Id
            );
            containers.Add(c5);
            products.AddRange(CreateProductsForContainer(ref productId, c5, random, itemCount: 7));
            UpdateContainerTotalsFromProducts(c5, products);
            telemetries.AddRange(CreateTelemetryReadings(
                ref telemetryId, c5, baseTelemetryDate,
                isTemperatureTooHigh: false, isTemperatureTooLow: false, isHumidityTooHigh: true,
                random
            ));

            // Container 6: Highly Perishable with HIGH humidity
            var c6 = CreateContainer(
                id: containerId++,
                containerNumber: "MSCU3001",
                category: MscCategoryCode.HighlyPerishable,
                agentCode: "AG003",
                bookingId: booking3.Id
            );
            containers.Add(c6);
            products.AddRange(CreateProductsForContainer(ref productId, c6, random, itemCount: 6));
            UpdateContainerTotalsFromProducts(c6, products);
            telemetries.AddRange(CreateTelemetryReadings(
                ref telemetryId, c6, baseTelemetryDate,
                isTemperatureTooHigh: false, isTemperatureTooLow: false, isHumidityTooHigh: true,
                random
            ));

            // -------------------------
            // BOOKING 4 (Agency 2004)
            // -------------------------
            var booking4 = CreateBooking(
                id: bookingId++,
                bkNumber: "BK-000004",
                agencyCode: "MSC-AG-2004",
                shippingDate: baseShippingDate.AddDays(3),
                customerName: "Elena",
                customerSurname: "Costa",
                customerTaxCode: "CSTLNE92H61F952Q",
                customerCompany: "Luxury Goods S.r.l.",
                consigneeName: "James",
                consigneeSurname: "Taylor",
                consigneeTaxCode: "TYLJMS90A01Z100C",
                consigneeCompany: "UK Import Ltd.",
                originNation: "IT",
                originCity: "Milano",
                originAddress: "Via Manzoni 5",
                originPostalCode: "20100",
                destinationNation: "GB",
                destinationCity: "London",
                destinationAddress: "Dock Road 12",
                destinationPostalCode: "E16 1AA"
            );
            bookings.Add(booking4);

            // Container 7: Pharmaceuticals - ALL VALUES OPTIMAL
            var c7 = CreateContainer(
                id: containerId++,
                containerNumber: "MSCU4000",
                category: MscCategoryCode.Pharmaceuticals,
                agentCode: "AG004",
                bookingId: booking4.Id
            );
            containers.Add(c7);
            products.AddRange(CreateProductsForContainer(ref productId, c7, random, itemCount: 6));
            UpdateContainerTotalsFromProducts(c7, products);
            telemetries.AddRange(CreateTelemetryReadings(
                ref telemetryId, c7, baseTelemetryDate,
                isTemperatureTooHigh: false, isTemperatureTooLow: false, isHumidityTooHigh: false,
                random
            ));

            // Container 8: Temperature Controlled - ALL VALUES OPTIMAL
            var c8 = CreateContainer(
                id: containerId++,
                containerNumber: "MSCU4001",
                category: MscCategoryCode.TemperatureControlled,
                agentCode: "AG004",
                bookingId: booking4.Id
            );
            containers.Add(c8);
            products.AddRange(CreateProductsForContainer(ref productId, c8, random, itemCount: 6));
            UpdateContainerTotalsFromProducts(c8, products);
            telemetries.AddRange(CreateTelemetryReadings(
                ref telemetryId, c8, baseTelemetryDate,
                isTemperatureTooHigh: false, isTemperatureTooLow: false, isHumidityTooHigh: false,
                random
            ));

            // -------------------------
            // Persist seed data (EF Core HasData)
            // -------------------------
            modelBuilder.Entity<Booking>().HasData(bookings);
            modelBuilder.Entity<Container>().HasData(containers);
            modelBuilder.Entity<Product>().HasData(products);
            modelBuilder.Entity<Telemetry>().HasData(telemetries);
        }

        private static Booking CreateBooking(
            int id,
            string bkNumber,
            string agencyCode,
            DateTime shippingDate,
            string customerName,
            string customerSurname,
            string customerTaxCode,
            string customerCompany,
            string consigneeName,
            string consigneeSurname,
            string consigneeTaxCode,
            string consigneeCompany,
            string originNation,
            string originCity,
            string originAddress,
            string originPostalCode,
            string destinationNation,
            string destinationCity,
            string destinationAddress,
            string destinationPostalCode)
        {
            return new Booking
            {
                Id = id,
                BkNumber = bkNumber,
                AgencyCode = agencyCode,
                ShippingDate = shippingDate,

                CustomerName = customerName,
                CustomerSurname = customerSurname,
                CustomerTaxCode = customerTaxCode,
                CustomerCompany = customerCompany,

                ConsigneeName = consigneeName,
                ConsigneeSurname = consigneeSurname,
                ConsigneeTaxCode = consigneeTaxCode,
                ConsigneeCompany = consigneeCompany,

                OriginNation = originNation,
                OriginCity = originCity,
                OriginAddress = originAddress,
                OriginPostalCode = originPostalCode,

                DestinationNation = destinationNation,
                DestinationCity = destinationCity,
                DestinationAddress = destinationAddress,
                DestinationPostalCode = destinationPostalCode,
            };
        }

        /// <summary>
        /// Helper method to create a container with consistent setpoints (similar to old seeder)
        /// </summary>
        private static Container CreateContainer(
            int id,
            string containerNumber,
            MscCategoryCode category,
            string agentCode,
            int bookingId)
        {
            // Ideal temperature setpoint by category
            double temperatureSetpoint = category switch
            {
                MscCategoryCode.Pharmaceuticals => 5.0,           // 2-8°C
                MscCategoryCode.HighlyPerishable => 1.0,          // -2 to +4°C
                MscCategoryCode.FreshProduce => 7.0,              // 2-12°C
                MscCategoryCode.Frozen => -21.0,                  // -18 to -25°C
                MscCategoryCode.TemperatureControlled => 18.0,    // 10-25°C
                _ => 5.0
            };

            // Ideal humidity setpoint by category
            double humiditySetpoint = category switch
            {
                MscCategoryCode.Pharmaceuticals => 60.0,
                MscCategoryCode.HighlyPerishable => 85.0,
                MscCategoryCode.FreshProduce => 90.0,
                MscCategoryCode.Frozen => 70.0,
                MscCategoryCode.TemperatureControlled => 55.0,
                _ => 70.0
            };

            return new Container
            {
                Id = id,
                ContainerNumber = containerNumber,
                ProductCategory = category,
                AgentCode = agentCode,
                BookingId = bookingId,

                TemperatureSetpoint = temperatureSetpoint,
                HumiditySetpoint = humiditySetpoint,

                // valorizzati dopo che creiamo i products
                TotalWeight = 0,
                TotalVolume = 0
            };
        }

        /// <summary>
        /// Create a small set of products for a container (EF HasData friendly: no navigation set).
        /// Keeps weights/volumes reasonable and under MAX_VOLUME total in most cases.
        /// </summary>
        private static List<Product> CreateProductsForContainer(
            ref int startId,
            Container container,
            Random random,
            int itemCount)
        {
            var list = new List<Product>();

            for (int i = 0; i < itemCount; i++)
            {
                // weights/volumes scaled by category (just to make data "look" plausible)
                (double minW, double maxW, double minV, double maxV, string baseName) = container.ProductCategory switch
                {
                    MscCategoryCode.Pharmaceuticals => (20, 120, 0.05, 0.25, "Pharma Batch"),
                    MscCategoryCode.HighlyPerishable => (200, 1200, 0.3, 2.0, "Perishable Lot"),
                    MscCategoryCode.FreshProduce => (250, 1500, 0.5, 2.5, "Fresh Produce"),
                    MscCategoryCode.Frozen => (300, 2000, 0.6, 3.0, "Frozen Goods"),
                    MscCategoryCode.TemperatureControlled => (150, 1800, 0.4, 2.8, "Temp-Controlled"),
                    _ => (100, 1000, 0.2, 2.0, "Generic")
                };

                var w = Round2(minW + random.NextDouble() * (maxW - minW));
                var v = Round2(minV + random.NextDouble() * (maxV - minV));

                list.Add(new Product
                {
                    Id = startId++,
                    ContainerId = container.Id,
                    CategoryCode = container.ProductCategory,
                    ProductName = $"{baseName} {i + 1}",
                    Notes = $"Seeded product for {container.ContainerNumber}",
                    Weight = w,
                    Volume = v,
                    IsDeleted = false
                });
            }

            return list;
        }

        /// <summary>
        /// Update container totals after products have been appended to the global list.
        /// </summary>
        private static void UpdateContainerTotalsFromProducts(Container container, List<Product> allProducts)
        {
            var mine = allProducts.Where(p => p.ContainerId == container.Id).ToList();
            container.TotalWeight = Round2(mine.Sum(p => p.Weight));
            container.TotalVolume = Round2(mine.Sum(p => p.Volume));
        }

        /// <summary>
        /// Creates telemetry readings for a container.
        /// Generates one reading every 12 hours for 5 days (10 readings total).
        /// </summary>
        private static List<Telemetry> CreateTelemetryReadings(
            ref int startId,
            Container container,
            DateTimeOffset startDate,
            bool isTemperatureTooHigh,
            bool isTemperatureTooLow,
            bool isHumidityTooHigh,
            Random random)
        {
            var list = new List<Telemetry>();
            var category = container.ProductCategory;

            double tempSetpoint = container.TemperatureSetpoint ?? 5.0;
            double humidSetpoint = container.HumiditySetpoint ?? 70.0;
            double tempTolerance = category.GetTemperatureTolerance();
            double humidTolerance = category.GetHumidityTolerance();

            for (int hour = 0; hour < 120; hour += 12)
            {
                var readingTime = startDate.AddHours(hour);

                double temperature;
                double humidity;

                if (isTemperatureTooHigh)
                {
                    temperature = tempSetpoint + tempTolerance + (random.NextDouble() * 2 + 0.5);
                    humidity = humidSetpoint + (random.NextDouble() * 2 - 1) * (humidTolerance * 0.5);
                }
                else if (isTemperatureTooLow)
                {
                    temperature = tempSetpoint - tempTolerance - (random.NextDouble() * 2 + 0.5);
                    humidity = humidSetpoint + (random.NextDouble() * 2 - 1) * (humidTolerance * 0.5);
                }
                else if (isHumidityTooHigh)
                {
                    temperature = tempSetpoint + (random.NextDouble() * 2 - 1) * (tempTolerance * 0.5);
                    humidity = humidSetpoint + humidTolerance + (random.NextDouble() * 10 + 2);
                }
                else
                {
                    temperature = tempSetpoint + (random.NextDouble() * 2 - 1) * (tempTolerance * 0.8);
                    humidity = humidSetpoint + (random.NextDouble() * 2 - 1) * (humidTolerance * 0.8);
                }

                humidity = Math.Clamp(humidity, 0, 100);

                list.Add(new Telemetry
                {
                    Id = startId++,
                    ContainerId = container.Id,
                    DateRegistered = readingTime,
                    Temperature = (float)temperature,
                    Humidity = (float)humidity
                });
            }

            return list;
        }

        private static double Round2(double v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);
    }
}
