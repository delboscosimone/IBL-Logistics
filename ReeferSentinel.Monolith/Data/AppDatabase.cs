using Microsoft.EntityFrameworkCore;
using ReeferSentinel.Monolith.Models;

namespace ReeferSentinel.Monolith.Data
{
    public class AppDatabase : DbContext
    {
        public AppDatabase(DbContextOptions<AppDatabase> options) : base(options)
        {
        }

        public DbSet<Container> Containers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Telemetry> Telemetries { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Port> Ports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Port>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Code).HasMaxLength(10);
                entity.Property(p => p.Name).HasMaxLength(120);
                entity.Property(p => p.City).HasMaxLength(120);
                entity.Property(p => p.Nation).HasMaxLength(10);
            });

            modelBuilder.Entity<Container>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(p => p.TotalWeight).HasDefaultValue(0.0);
                entity.Property(p => p.TotalVolume).HasDefaultValue(0.0);

                entity.HasMany(c => c.Telemetries)
                    .WithOne(t => t.Container)
                    .HasForeignKey(t => t.ContainerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Products)
                    .WithOne(p => p.Container)
                    .HasForeignKey(p => p.ContainerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.CurrentPort)
                    .WithMany(p => p.Containers)
                    .HasForeignKey(c => c.CurrentPortId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ProductName).HasDefaultValue("N/D");
                entity.Property(p => p.Notes).HasDefaultValue("N/D");
                entity.Property(p => p.Weight).HasDefaultValue(0.0);
                entity.Property(p => p.Volume).HasDefaultValue(0.0);
                entity.Property(p => p.IsDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<Telemetry>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.DateRegistered).HasColumnType("datetime(6)");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.HasMany(b => b.Containers)
                    .WithOne(c => c.Booking)
                    .HasForeignKey(c => c.BookingId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(b => b.OriginPort)
                    .WithMany(p => p.OriginBookings)
                    .HasForeignKey(b => b.OriginPortId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(b => b.DestinationPort)
                    .WithMany(p => p.DestinationBookings)
                    .HasForeignKey(b => b.DestinationPortId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            DatabaseSeeder.SeedDatabase(modelBuilder);
        }

        public async Task<int> CreateEmptyContainer(
            string containerNumber,
            string agencyCode,
            string agentCode,
            MscCategoryCode categoryCode,
            string destination = null,
            int? currentPortId = null)
        {
            var container = new Container
            {
                ContainerNumber = containerNumber,
                AgentCode = agentCode,
                ProductCategory = categoryCode,
                TotalWeight = 0,
                TotalVolume = 0,
                TemperatureSetpoint = GetTemperatureSetpoint(categoryCode),
                HumiditySetpoint = GetHumiditySetpoint(categoryCode),
                CurrentPortId = currentPortId
            };

            await Containers.AddAsync(container);
            await SaveChangesAsync();
            return container.Id;
        }

        public async Task<int> CreateNewBooking(
            string BkNumber,
            string CustomerName,
            string CustomerSurname,
            string CustomerTaxCode,
            string CustomerCompany,
            string ConsigneeName,
            string ConsigneeSurname,
            string ConsigneeTaxCode,
            string ConsigneeCompany,
            string OriginNation,
            string OriginCity,
            string OriginAddress,
            string OriginPostalCode,
            string DestinationNation,
            string DestinationCity,
            string DestinationAddress,
            string DestinationPostalCode,
            string agencyCode,
            DateTime ShippingDate,
            DateTime EstimatedArrivalDate,
            int? originPortId = null,
            int? destinationPortId = null)
        {
            var booking = new Booking
            {
                BkNumber = BkNumber,
                CustomerName = CustomerName,
                CustomerSurname = CustomerSurname,
                CustomerTaxCode = CustomerTaxCode,
                CustomerCompany = CustomerCompany,
                ConsigneeName = ConsigneeName,
                ConsigneeSurname = ConsigneeSurname,
                ConsigneeTaxCode = ConsigneeTaxCode,
                ConsigneeCompany = ConsigneeCompany,
                OriginNation = OriginNation,
                OriginCity = OriginCity,
                OriginAddress = OriginAddress,
                OriginPostalCode = OriginPostalCode,
                DestinationNation = DestinationNation,
                DestinationCity = DestinationCity,
                DestinationAddress = DestinationAddress,
                DestinationPostalCode = DestinationPostalCode,
                AgencyCode = agencyCode,
                ShippingDate = ShippingDate,
                EstimatedArrivalDate = EstimatedArrivalDate,
                OriginPortId = originPortId,
                DestinationPortId = destinationPortId
            };

            await Bookings.AddAsync(booking);
            await SaveChangesAsync();
            return booking.Id;
        }

        public async Task<int> AssociateContainers(int containerId, int bookingId, string agencyCode)
        {
            var container = await Containers.SingleOrDefaultAsync(c => c.Id == containerId)
                ?? throw new ArgumentException($"Container with ID {containerId} not found");

            var booking = await Bookings.SingleOrDefaultAsync(b => b.Id == bookingId)
                ?? throw new ArgumentException($"Booking with ID {bookingId} not found");

            if (container.BookingId.HasValue && container.BookingId.Value != bookingId)
            {
                throw new InvalidOperationException($"Il container {container.ContainerNumber} è già associato a un altro booking.");
            }

            if (container.CurrentPortId.HasValue && booking.OriginPortId.HasValue && container.CurrentPortId.Value != booking.OriginPortId.Value)
            {
                throw new InvalidOperationException($"Il container {container.ContainerNumber} non si trova nel porto di partenza selezionato.");
            }

            container.BookingId = booking.Id;
            await SaveChangesAsync();
            return booking.Id;
        }

        public async Task<int> AddProductsToContainer(int containerId, string productName, string notes, MscCategoryCode categoryCode, double productWeight, double productVolume, string agentCode)
        {
            var container = await Containers.Include(i => i.Products).Where(c => c.Id == containerId).FirstOrDefaultAsync();
            if (container == null)
            {
                throw new ArgumentException($"Container with ID {containerId} not found");
            }

            AuthorizationCheck(agentCode, container.AgentCode);

            var product = new Product
            {
                ContainerId = containerId,
                CategoryCode = categoryCode,
                ProductName = productName,
                Notes = notes,
                Weight = productWeight,
                Volume = productVolume,
                IsDeleted = false
            };

            await Products.AddAsync(product);
            container.TotalWeight += productWeight;
            container.TotalVolume += productVolume;
            await SaveChangesAsync();
            return product.Id;
        }

        public async Task<ContainerInfo> GetContainerById(int containerId)
        {
            var container = await Containers.Include(c => c.CurrentPort).Where(c => c.Id == containerId).FirstOrDefaultAsync();
            if (container == null)
            {
                return null;
            }

            return new ContainerInfo
            {
                Id = container.Id,
                ContainerNumber = container.ContainerNumber,
                ProductCategoryCode = (int?)container.ProductCategory,
                ProductCategoryName = container.ProductCategory.ToString(),
                TotalWeight = container.TotalWeight,
                TotalVolume = container.TotalVolume,
                AgentCode = container.AgentCode,
                TemperatureSetpoint = container.TemperatureSetpoint,
                HumiditySetpoint = container.HumiditySetpoint,
                CurrentPortId = container.CurrentPortId,
                CurrentPortCode = container.CurrentPort?.Code,
                CurrentPortName = container.CurrentPort is null ? null : $"{container.CurrentPort.Name} ({container.CurrentPort.Nation})",
                IsAvailable = container.BookingId == null
            };
        }

        public Task<ProductSummary> GetContainerProductsSummary(int containerId, string agentCode)
        {
            var container = Containers.Include(p => p.Products).Where(c => c.Id == containerId).FirstOrDefault();
            if (container == null)
            {
                return Task.FromResult<ProductSummary>(null);
            }

            AuthorizationCheck(agentCode, container.AgentCode);

            var count = container.Products?.Count(p => !p.IsDeleted) ?? 0;
            var availableVolume = Container.MAX_VOLUME - container.TotalVolume;
            var summary = new ProductSummary
            {
                ContainerNumber = container.ContainerNumber,
                ProductCategoryName = container.ProductCategory.ToString(),
                AgencyCode = string.Empty,
                AgentCode = container.AgentCode,
                TotalWeight = container.TotalWeight,
                TotalVolume = container.TotalVolume,
                AvailableVolume = availableVolume,
                VolumeUtilizationPercentage = container.TotalVolume / Container.MAX_VOLUME * 100,
                Count = count
            };

            return Task.FromResult(summary);
        }

        public async Task<List<SpecificProductSummary>> GetAllProductsDetails(int containerId, string agentCode)
        {
            var container = await Containers.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == containerId);
            if (container == null)
            {
                return null;
            }

            AuthorizationCheck(agentCode, container.AgentCode);
            return container.Products
                .Where(p => !p.IsDeleted)
                .Select(product => new SpecificProductSummary
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    ProductCategoryName = product.CategoryCode.ToString(),
                    Weight = product.Weight,
                    Volume = product.Volume,
                    Notes = product.Notes,
                    IsDeleted = product.IsDeleted
                })
                .ToList();
        }

        public BookingSummary GetBookingById(int bookingId)
        {
            var booking = Bookings
                .Include(i => i.Containers)
                .Include(i => i.OriginPort)
                .Include(i => i.DestinationPort)
                .Where(c => c.Id == bookingId)
                .SingleOrDefault();

            if (booking == null)
            {
                return null;
            }

            var bookingContainers = booking.Containers.Select(container => new SpecificContainerSummary
            {
                Id = container.Id,
                ContainerNumber = container.ContainerNumber,
                ProductCategory = container.ProductCategory,
                TotalWeight = container.TotalWeight,
                TotalVolume = container.TotalVolume
            }).ToList();

            return new BookingSummary
            {
                Id = booking.Id,
                BookingNumber = booking.BkNumber,
                ShippingDate = booking.ShippingDate,
                EstimatedArrivalDate = booking.EstimatedArrivalDate,
                OriginPortDisplay = booking.OriginPort is null ? null : $"{booking.OriginPort.Name} ({booking.OriginPort.Nation}) - {booking.OriginPort.Code}",
                DestinationPortDisplay = booking.DestinationPort is null ? null : $"{booking.DestinationPort.Name} ({booking.DestinationPort.Nation}) - {booking.DestinationPort.Code}",
                CustomerInformations = $"{booking.CustomerName} {booking.CustomerSurname} {booking.CustomerTaxCode} - {booking.CustomerCompany}",
                OriginAdress = $"{booking.OriginNation}, {booking.OriginCity}, {booking.OriginAddress}, {booking.OriginPostalCode}",
                ConsigneeInformations = $"{booking.ConsigneeName} {booking.ConsigneeSurname} {booking.ConsigneeTaxCode} - {booking.ConsigneeCompany}",
                DestinationAdress = $"{booking.DestinationNation}, {booking.DestinationCity}, {booking.DestinationAddress}, {booking.DestinationPostalCode}",
                ContainerSummaries = bookingContainers
            };
        }

        public List<CategoryInfo> GetAllCategoryDetails()
        {
            return Enum.GetValues<MscCategoryCode>().Select(code => new CategoryInfo
            {
                Code = (int)code,
                Description = code.ToString(),
                DefaultTemperatureSetpoint = GetTemperatureSetpoint(code),
                DefaultHumiditySetpoint = GetHumiditySetpoint(code),
                IconUrl = GetCategoryIconUrl(code)
            }).ToList();
        }

        public async Task<float?> GetAverageTemperature(int containerId, string agentCode)
        {
            var container = await Containers.Include(c => c.Telemetries).FirstOrDefaultAsync(c => c.Id == containerId);
            if (container == null)
            {
                return null;
            }

            AuthorizationCheck(agentCode, container.AgentCode);
            return container.Telemetries.Any() ? container.Telemetries.Average(t => t.Temperature) : null;
        }

        public async Task<List<int>> GetAllContainerIds()
        {
            return await Containers.Select(c => c.Id).ToListAsync();
        }

        public async Task ChangeTemperatureAndHumidity(int containerId, string agentCode, double? newTemperatureSetpoint, double? newHumiditySetpoint)
        {
            var container = await Containers.Where(c => c.Id == containerId).FirstOrDefaultAsync();
            if (container == null)
            {
                throw new ArgumentException($"Container with ID {containerId} not found");
            }

            AuthorizationCheck(agentCode, container.AgentCode);
            if (newHumiditySetpoint < 0)
            {
                throw new ArgumentException("Humidity can't accept negative values");
            }

            if (newHumiditySetpoint is null && newTemperatureSetpoint is null)
            {
                throw new ArgumentException("Insert at least one value!");
            }

            if (newTemperatureSetpoint != null)
            {
                container.TemperatureSetpoint = newTemperatureSetpoint;
            }
            if (newHumiditySetpoint != null)
            {
                container.HumiditySetpoint = newHumiditySetpoint;
            }

            await SaveChangesAsync();
        }

        public List<string> GetContainerOutOfRangeByBookingId(int bookingId)
        {
            var containers = Containers.Include(c => c.Telemetries).Where(i => i.BookingId == bookingId).ToList();
            if (!containers.Any())
            {
                return null;
            }

            var containersOutOfRange = new List<string>();
            foreach (var container in containers)
            {
                var tempTolerance = MscCategoryCodeExtensions.GetTemperatureTolerance(container.ProductCategory);
                if (container.Telemetries.Any(telemetry => telemetry.Temperature < (container.TemperatureSetpoint - tempTolerance) || telemetry.Temperature > (container.TemperatureSetpoint + tempTolerance)))
                {
                    containersOutOfRange.Add($"{container.Id} - {container.ContainerNumber}");
                }
            }

            return containersOutOfRange.Distinct().ToList();
        }

        public List<string> GetTelemetriesOutOfRange(int containerId)
        {
            var container = Containers.Include(c => c.Telemetries).Where(c => c.Id == containerId).SingleOrDefault();
            if (container == null)
            {
                return null;
            }

            var tempTolerance = MscCategoryCodeExtensions.GetTemperatureTolerance(container.ProductCategory);
            var temperatureSetpoint = container.TemperatureSetpoint;
            return container.Telemetries
                .Where(t => IsOutOfRange(t, tempTolerance, temperatureSetpoint))
                .DistinctBy(d => d.DateRegistered)
                .Select(s => $"{container.Id} - {s.DateRegistered:yyyy-MM-dd HH:mm} - {s.Temperature} °C")
                .ToList();
        }

        public async Task<string> RemoveProduct(bool softOrHardDelete, string agentCode, int containerId, string productName, double weight, double volume)
        {
            var container = await Containers.Where(c => c.Id == containerId).FirstOrDefaultAsync();
            if (container == null)
            {
                throw new ArgumentException($"Container with ID {containerId} not found");
            }

            AuthorizationCheck(agentCode, container.AgentCode);
            var product = await Products.Where(c => c.ContainerId == containerId && c.ProductName == productName && c.Weight == weight && c.Volume == volume).FirstOrDefaultAsync();
            if (product == null || product.IsDeleted)
            {
                throw new Exception("Product not found or already deleted");
            }

            if (!softOrHardDelete)
            {
                product.IsDeleted = true;
            }
            else
            {
                Products.Remove(product);
            }

            container.TotalVolume = await Products.Where(p => p.ContainerId == containerId && !p.IsDeleted && p.Id != product.Id).SumAsync(s => (double?)s.Volume) ?? 0;
            container.TotalWeight = await Products.Where(p => p.ContainerId == containerId && !p.IsDeleted && p.Id != product.Id).SumAsync(s => (double?)s.Weight) ?? 0;
            await SaveChangesAsync();
            return productName;
        }

        public List<PortInfo> GetPorts()
        {
            return Ports.Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Select(p => new PortInfo { Id = p.Id, Code = p.Code, Name = p.Name, Nation = p.Nation, City = p.City })
                .ToList();
        }

        public List<ContainerInfo> GetAvailableContainersForPort(int portId, DateTime shippingDate)
        {
            return Containers
                .Include(c => c.CurrentPort)
                .Include(c => c.Booking)
                .Where(c => c.CurrentPortId == portId &&
                            (c.BookingId == null || c.Booking == null || c.Booking.EstimatedArrivalDate < shippingDate))
                .OrderBy(c => c.ContainerNumber)
                .Select(container => new ContainerInfo
                {
                    Id = container.Id,
                    ContainerNumber = container.ContainerNumber,
                    ProductCategoryCode = (int)container.ProductCategory,
                    ProductCategoryName = container.ProductCategory.ToString(),
                    TotalWeight = container.TotalWeight,
                    TotalVolume = container.TotalVolume,
                    AgentCode = container.AgentCode,
                    TemperatureSetpoint = container.TemperatureSetpoint,
                    HumiditySetpoint = container.HumiditySetpoint,
                    CurrentPortId = container.CurrentPortId,
                    CurrentPortCode = container.CurrentPort != null ? container.CurrentPort.Code : null,
                    CurrentPortName = container.CurrentPort != null ? $"{container.CurrentPort.Name} ({container.CurrentPort.Nation})" : null,
                    IsAvailable = true
                })
                .ToList();
        }

        private bool IsOutOfRange(Telemetry telemetry, double tempTolerance, double? temperatureSetpoint)
        {
            return telemetry.Temperature < (temperatureSetpoint - tempTolerance) || telemetry.Temperature > (temperatureSetpoint + tempTolerance);
        }

        private double GetTemperatureSetpoint(MscCategoryCode category)
        {
            return category switch
            {
                MscCategoryCode.Pharmaceuticals => 5.0,
                MscCategoryCode.HighlyPerishable => 1.0,
                MscCategoryCode.FreshProduce => 7.0,
                MscCategoryCode.Frozen => -21.0,
                MscCategoryCode.TemperatureControlled => 18.0,
                _ => 5.0
            };
        }

        private double GetHumiditySetpoint(MscCategoryCode category)
        {
            return category switch
            {
                MscCategoryCode.Pharmaceuticals => 60.0,
                MscCategoryCode.HighlyPerishable => 85.0,
                MscCategoryCode.FreshProduce => 90.0,
                MscCategoryCode.Frozen => 70.0,
                MscCategoryCode.TemperatureControlled => 55.0,
                _ => 70.0
            };
        }

        private string GetCategoryIconUrl(MscCategoryCode category)
        {
            return category switch
            {
                MscCategoryCode.Pharmaceuticals => "/icons/pharmaceuticals.svg",
                MscCategoryCode.HighlyPerishable => "/icons/highly-perishable.svg",
                MscCategoryCode.FreshProduce => "/icons/fresh-produce.svg",
                MscCategoryCode.Frozen => "/icons/frozen.svg",
                MscCategoryCode.TemperatureControlled => "/icons/temperature-controlled.svg",
                _ => "/icons/container-default.svg"
            };
        }

        public void AuthorizationCheck(string agencyCode, string DBAgencyCode)
        {
            if (!string.Equals(agencyCode, DBAgencyCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"This agency code : {agencyCode} doesn't have the authorization to perform this action!");
            }
        }
    }
}
