using Microsoft.EntityFrameworkCore;
using ReeferSentinel.Monolith.Models;

namespace ReeferSentinel.Monolith.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedDatabase(ModelBuilder modelBuilder)
        {
            var ports = new List<Port>
            {
                new() { Id = 1, Code = "GOA", Name = "Genova", City = "Genova", Nation = "ITA", IsActive = true },
                new() { Id = 2, Code = "NAP", Name = "Napoli", City = "Napoli", Nation = "ITA", IsActive = true },
                new() { Id = 3, Code = "LIV", Name = "Livorno", City = "Livorno", Nation = "ITA", IsActive = true },
                new() { Id = 4, Code = "LAX", Name = "Los Angeles", City = "Los Angeles", Nation = "USA", IsActive = true },
                new() { Id = 5, Code = "MIA", Name = "Miami", City = "Miami", Nation = "USA", IsActive = true },
                new() { Id = 6, Code = "TNG", Name = "Tangeri Med", City = "Tangeri Med", Nation = "MAR", IsActive = true },
                new() { Id = 7, Code = "SSZ", Name = "San Paolo", City = "Santos / San Paolo", Nation = "BRA", IsActive = true },
                new() { Id = 8, Code = "BCN", Name = "Barcellona", City = "Barcellona", Nation = "ESP", IsActive = true },
                new() { Id = 9, Code = "TYO", Name = "Tokio", City = "Tokio", Nation = "JPN", IsActive = true },
                new() { Id = 10, Code = "HKG", Name = "Hong Kong", City = "Hong Kong", Nation = "CHN", IsActive = true }
            };
            modelBuilder.Entity<Port>().HasData(ports);

            var bookings = new List<Booking>
            {
                new()
                {
                    Id = 1, BkNumber = "BK-000001", AgencyCode = "MSC-AG-2001", ShippingDate = new DateTime(2026, 4, 12), EstimatedArrivalDate = new DateTime(2026, 4, 20),
                    CustomerName = "Aurora", CustomerSurname = "Foods", CustomerTaxCode = "RRAFLD00A01F205X", CustomerCompany = "Aurora Global Foods",
                    ConsigneeName = "James", ConsigneeSurname = "Taylor", ConsigneeTaxCode = "TYLJMS90A01Z100C", ConsigneeCompany = "West Coast Import LLC",
                    OriginNation = "ITA", OriginCity = "Genova", OriginAddress = "Terminal Sech", OriginPostalCode = "16126",
                    DestinationNation = "USA", DestinationCity = "Miami", DestinationAddress = "Dodge Island", DestinationPostalCode = "33132",
                    OriginPortId = 1, DestinationPortId = 5
                },
                new()
                {
                    Id = 2, BkNumber = "BK-000002", AgencyCode = "MSC-AG-2002", ShippingDate = new DateTime(2026, 4, 14), EstimatedArrivalDate = new DateTime(2026, 4, 25),
                    CustomerName = "Blue", CustomerSurname = "Med", CustomerTaxCode = "BLUMED00A01F205X", CustomerCompany = "Blue Med Pharma",
                    ConsigneeName = "Sophie", ConsigneeSurname = "Dubois", ConsigneeTaxCode = "DBSSPH88F01Z110B", ConsigneeCompany = "Distribution FR SAS",
                    OriginNation = "ITA", OriginCity = "Napoli", OriginAddress = "Molo Beverello", OriginPostalCode = "80133",
                    DestinationNation = "ESP", DestinationCity = "Barcellona", DestinationAddress = "Moll de Barcelona", DestinationPostalCode = "08039",
                    OriginPortId = 2, DestinationPortId = 8
                },
                new()
                {
                    Id = 3, BkNumber = "BK-000003", AgencyCode = "MSC-AG-2003", ShippingDate = new DateTime(2026, 4, 15), EstimatedArrivalDate = new DateTime(2026, 4, 28),
                    CustomerName = "Pacific", CustomerSurname = "Retail", CustomerTaxCode = "PCFRTL00A01F205X", CustomerCompany = "Pacific Retail Co.",
                    ConsigneeName = "Ming", ConsigneeSurname = "Lee", ConsigneeTaxCode = "LEEMNG00A01H123X", ConsigneeCompany = "Hong Kong Trade Hub",
                    OriginNation = "USA", OriginCity = "Los Angeles", OriginAddress = "Pier 400", OriginPostalCode = "90731",
                    DestinationNation = "CHN", DestinationCity = "Hong Kong", DestinationAddress = "Kwai Tsing Terminal", DestinationPostalCode = "00000",
                    OriginPortId = 4, DestinationPortId = 10
                },
                new()
                {
                    Id = 4, BkNumber = "BK-000004", AgencyCode = "MSC-AG-2004", ShippingDate = new DateTime(2026, 4, 18), EstimatedArrivalDate = new DateTime(2026, 5, 2),
                    CustomerName = "Luxury", CustomerSurname = "Group", CustomerTaxCode = "LXRGRP00A01F205X", CustomerCompany = "Luxury Group Italia",
                    ConsigneeName = "Rafael", ConsigneeSurname = "Costa", ConsigneeTaxCode = "CSTRFL00A01F205X", ConsigneeCompany = "Brasil Premium Imports",
                    OriginNation = "ITA", OriginCity = "Livorno", OriginAddress = "Darsena Toscana", OriginPostalCode = "57123",
                    DestinationNation = "BRA", DestinationCity = "San Paolo", DestinationAddress = "Porto de Santos", DestinationPostalCode = "11013-540",
                    OriginPortId = 3, DestinationPortId = 7
                }
            };
            modelBuilder.Entity<Booking>().HasData(bookings);

            var containers = new List<Container>
            {
                CreateContainer(1, "MSCU1000", MscCategoryCode.Pharmaceuticals, "AG001", 1, 1),
                CreateContainer(2, "MSCU1001", MscCategoryCode.HighlyPerishable, "AG001", 1, 1),
                CreateContainer(3, "MSCU2000", MscCategoryCode.FreshProduce, "AG002", 2, 2),
                CreateContainer(4, "MSCU2001", MscCategoryCode.Frozen, "AG002", 2, 2),
                CreateContainer(5, "MSCU3000", MscCategoryCode.FreshProduce, "AG003", 4, 3),
                CreateContainer(6, "MSCU3001", MscCategoryCode.HighlyPerishable, "AG003", 4, 3),
                CreateContainer(7, "MSCU4000", MscCategoryCode.Pharmaceuticals, "AG004", 3, 4),
                CreateContainer(8, "MSCU4001", MscCategoryCode.TemperatureControlled, "AG004", 3, 4),
                CreateContainer(9, "MSCU5000", MscCategoryCode.Frozen, "AG005", 1, null),
                CreateContainer(10, "MSCU5001", MscCategoryCode.FreshProduce, "AG005", 1, null),
                CreateContainer(11, "MSCU5002", MscCategoryCode.Pharmaceuticals, "AG006", 2, null),
                CreateContainer(12, "MSCU5003", MscCategoryCode.TemperatureControlled, "AG006", 3, null),
                CreateContainer(13, "MSCU5004", MscCategoryCode.HighlyPerishable, "AG007", 4, null),
                CreateContainer(14, "MSCU5005", MscCategoryCode.Frozen, "AG008", 8, null)
            };
            modelBuilder.Entity<Container>().HasData(containers);

            var products = new List<Product>();
            int productId = 1;
            foreach (var container in containers)
            {
                var count = container.Id <= 8 ? 3 : 1;
                for (var i = 0; i < count; i++)
                {
                    products.Add(new Product
                    {
                        Id = productId++,
                        ContainerId = container.Id,
                        CategoryCode = container.ProductCategory,
                        ProductName = $"Cargo {container.ContainerNumber}-{i + 1}",
                        Notes = "Seeded cargo item",
                        Weight = 800 + (container.Id * 10) + (i * 25),
                        Volume = 2.4 + i,
                        IsDeleted = false
                    });
                }
            }
            modelBuilder.Entity<Product>().HasData(products);

            var telemetries = new List<Telemetry>();
            int telemetryId = 1;
            foreach (var container in containers)
            {
                for (var h = 0; h < 6; h++)
                {
                    telemetries.Add(new Telemetry
                    {
                        Id = telemetryId++,
                        ContainerId = container.Id,
                        DateRegistered = new DateTime(2026, 4, 1).AddHours(h),
                        Temperature = (float)(GetTemperature(container.ProductCategory) + ((h % 2 == 0) ? 0.5 : -0.4)),
                        Humidity = (float)(GetHumidity(container.ProductCategory) + ((h % 2 == 0) ? 1.0 : -1.0))
                    });
                }
            }
            modelBuilder.Entity<Telemetry>().HasData(telemetries);
        }

        private static Container CreateContainer(int id, string number, MscCategoryCode category, string agentCode, int currentPortId, int? bookingId)
        {
            return new Container
            {
                Id = id,
                ContainerNumber = number,
                ProductCategory = category,
                TotalWeight = bookingId.HasValue ? 2500 + (id * 35) : 0,
                TotalVolume = bookingId.HasValue ? 9 + (id % 3) : 0,
                AgentCode = agentCode,
                TemperatureSetpoint = GetTemperature(category),
                HumiditySetpoint = GetHumidity(category),
                CurrentPortId = currentPortId,
                BookingId = bookingId
            };
        }

        private static double GetTemperature(MscCategoryCode category) => category switch
        {
            MscCategoryCode.Pharmaceuticals => 5.0,
            MscCategoryCode.HighlyPerishable => 1.0,
            MscCategoryCode.FreshProduce => 7.0,
            MscCategoryCode.Frozen => -21.0,
            MscCategoryCode.TemperatureControlled => 18.0,
            _ => 5.0
        };

        private static double GetHumidity(MscCategoryCode category) => category switch
        {
            MscCategoryCode.Pharmaceuticals => 60.0,
            MscCategoryCode.HighlyPerishable => 85.0,
            MscCategoryCode.FreshProduce => 90.0,
            MscCategoryCode.Frozen => 70.0,
            MscCategoryCode.TemperatureControlled => 55.0,
            _ => 70.0
        };
    }
}
