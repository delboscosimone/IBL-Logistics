using Microsoft.EntityFrameworkCore;
using ReeferSentinel.Monolith.Models;


namespace ReeferSentinel.Monolith.Data
{
    /// <summary>
    /// Database context - handles all database operations for containers and telemetry data.
    /// This is the main connection between your code and the database.
    /// Also contains all business logic for managing containers.
    /// </summary>
    public class AppDatabase : DbContext
    {
        public AppDatabase(DbContextOptions<AppDatabase> options) : base(options)
        {
        }

        /// <summary>
        /// Table for storing containers
        /// </summary>
        public DbSet<Container> Containers { get; set; }

        /// <summary>
        /// Table for storing products
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Table for storing telemetry readings
        /// </summary>
        public DbSet<Telemetry> Telemetries { get; set; }
        public DbSet<Booking> Bookings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Container table
            modelBuilder.Entity<Container>(entity =>
            {
                // Set Id as the primary key
                entity.HasKey(c => c.Id);

                // Set default values for new containers
                entity.Property(p => p.TotalWeight).HasDefaultValue(0.0);
                entity.Property(p => p.TotalVolume).HasDefaultValue(0.0);

                // Configure relationship: one Container can have many Telemetries
                // When a Container is deleted, all its Telemetries are also deleted
                entity.HasMany(c => c.Telemetries)
                   .WithOne(t => t.Container)
                   .HasForeignKey(t => t.ContainerId)
                   .OnDelete(DeleteBehavior.Cascade);

                // Configure relationship: one Container can have many Products
                // When a Container is deleted, all its Products are also deleted
                entity.HasMany(c => c.Products)
                   .WithOne(p => p.Container)
                   .HasForeignKey(p => p.ContainerId)
                   .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Product table
            modelBuilder.Entity<Product>(entity =>
            {
                // Set Id as the primary key
                entity.HasKey(p => p.Id);

                // Set default values for new products
                entity.Property(p => p.ProductName).HasDefaultValue("N/D");
                entity.Property(p => p.Notes).HasDefaultValue("N/D");
                entity.Property(p => p.Weight).HasDefaultValue(0.0);
                entity.Property(p => p.Volume).HasDefaultValue(0.0);
                entity.Property(p => p.IsDeleted).HasDefaultValue(false); // soft delete (false = active, true = disable)
            });

            // Configure Telemetry table
            modelBuilder.Entity<Telemetry>(entity =>
                    {
                        // Set Id as the primary key
                        entity.HasKey(t => t.Id);

                        // Store DateRegistered with timezone information
                        entity.Property(t => t.DateRegistered).HasColumnType("datetimeoffset");
                    });

            // Add sample data to the database for testing
            DatabaseSeeder.SeedDatabase(modelBuilder);

            modelBuilder.Entity<Booking>(entity =>
            {
                // Set Id as the primary key
                entity.HasKey(b => b.Id);

                // Set default values for new containers
                entity.Property(b => b.BkNumber).HasDefaultValue(null);
                entity.Property(b => b.AgencyCode).HasDefaultValue(null);
                entity.Property(b => b.ShippingDate).HasDefaultValue(null);

                entity.Property(b => b.OriginNation).HasDefaultValue(null);
                entity.Property(b => b.OriginCity).HasDefaultValue(null);
                entity.Property(b => b.OriginAddress).HasDefaultValue(null);
                entity.Property(b => b.OriginPostalCode).HasDefaultValue(null);

                entity.Property(b => b.DestinationNation).HasDefaultValue(null);
                entity.Property(b => b.DestinationCity).HasDefaultValue(null);
                entity.Property(b => b.DestinationAddress).HasDefaultValue(null);
                entity.Property(b => b.DestinationPostalCode).HasDefaultValue(null);

                entity.Property(b => b.CustomerName).HasDefaultValue(null);
                entity.Property(b => b.CustomerSurname).HasDefaultValue(null);
                entity.Property(b => b.CustomerCompany).HasDefaultValue(null);
                entity.Property(b => b.CustomerTaxCode).HasDefaultValue(null);

                entity.Property(b => b.ConsigneeName).HasDefaultValue(null);
                entity.Property(b => b.ConsigneeSurname).HasDefaultValue(null);
                entity.Property(b => b.ConsigneeCompany).HasDefaultValue(null);
                entity.Property(b => b.ConsigneeTaxCode).HasDefaultValue(null);

                // Configure relationship: one Container can have many Telemetries
                // When a Container is deleted, all its Telemetries are also deleted
                entity.HasMany(b => b.Containers)
                   .WithOne(c => c.Booking)
                   .HasForeignKey(c => c.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);
            });
        }

        /// <summary>
        /// Creates a new empty container in the database
        /// </summary>
        /// <returns>The ID of the newly created container</returns>
        public async Task<int> CreateEmptyContainer(
            string containerNumber,
            string agencyCode,
            string agentCode,
            MscCategoryCode categoryCode,
            string destination = null)
        {


            // Create a new container with zero products
            var container = new Container
            {
                ContainerNumber = containerNumber,
                AgentCode = agentCode,
                ProductCategory = categoryCode,
                TotalWeight = 0,
                TotalVolume = 0,
                // Set temperature and humidity based on product category
                TemperatureSetpoint = GetTemperatureSetpoint(categoryCode),
                HumiditySetpoint = GetHumiditySetpoint(categoryCode)
            };

            // Add to database and save
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
            DateTime ShippingDate)
        {


            // Create a new container with zero products
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
                ShippingDate = ShippingDate
            };

            // Add to database and save
            await Bookings.AddAsync(booking);
            await SaveChangesAsync();

            return booking.Id;
        }

        /// <summary>
        /// Associate containers with booking
        /// </summary>
        public async Task<int> AssociateContainers(
            int containerId,
            int bookingId,
            string agencyCode)
        {
            var container = await Containers
                .SingleOrDefaultAsync(c => c.Id == containerId);

            if (container == null)
                throw new ArgumentException($"Container with ID {containerId} not found");


            var booking = await Bookings
                .SingleOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                throw new ArgumentException($"Booking with ID {bookingId} not found");

            container.BookingId = booking.Id;

            await SaveChangesAsync();

            return booking.Id;
        }




        /// <summary>
        /// Adds products to an existing container
        /// </summary>
        public async Task<int> AddProductsToContainer(
            int containerId,
            string productName,
            string notes,
            MscCategoryCode categoryCode,
            double productWeight,
            double productVolume,
            string agentCode)
        {
            // Find the container

            var container = await Containers.Include(i => i.Products)
                .Where(c => c.Id == containerId).FirstOrDefaultAsync();

            AuthorizationCheck(agentCode, container.AgentCode);

            if (container.ProductCategory != categoryCode)
            {
                throw new ArgumentException("Category mismatch");
            }

            if (container == null)
            {
                throw new ArgumentException($"Container with ID {containerId} not found");
            }

            // Not negative number
            if (productVolume < 0 || productWeight < 0)
            {
                throw new ArgumentException("You can't insert negative values.");
            }

            // Update product quantities
            container.TotalWeight += productWeight;
            container.TotalVolume += productVolume;

            // MAx volume
            if (container.TotalVolume >= Container.MAX_VOLUME)
            {
                throw new ArgumentException($"Container with ID {containerId} exceeds maximum volume");

            }
            else
            {
                await SaveChangesAsync();
            }

            // Create a new container with zero products
            var product = new Product
            {
                ContainerId = containerId,
                CategoryCode = categoryCode,
                ProductName = productName,
                Notes = notes,
                Weight = productWeight,
                Volume = productVolume
            };

            // Add to database and save
            await Products.AddAsync(product);
            await SaveChangesAsync();

            return product.Id;
        }

        //public async Task<int> ChangeProductLocation(int containerId, int newContainerId, int productId, string agentCode)
        //{
        //    var container = Containers.Where(c => c.Id == containerId).SingleOrDefault();
        //    AuthorizationCheck(agentCode, container.AgentCode);

        //    var newContainer = Containers.Where(c => c.Id == newContainerId).SingleOrDefault();
        //    AuthorizationCheck(agentCode, newContainer.AgentCode);

        //    var product = Products.Where(p => p.Id == productId).SingleOrDefault();

        //    if(product == null)
        //    {
        //        throw new ArgumentException("The product doesn't exist");
        //    }

        //    if (container == null || newContainer == null)
        //    {
        //        throw new ArgumentException("One or both containers are null");
        //    } 


        //    if (product.ContainerId != containerId && product.ContainerId != newContainerId)
        //    {
        //        throw new Exception("this products is not inside neither of these containers");
        //    }


        //    // container.Products.Where(p => p.Id == productId).Select(p => p.Volume).SingleOrDefault() 


        //    if ((newContainer.TotalVolume + product.Volume) < Container.MAX_VOLUME)
        //    {
        //        newContainer.Products.Add(product);
        //        container.Products.Remove(product);

        //        newContainer.TotalVolume += product.Volume;
        //        container.TotalVolume -= product.Volume;

        //        product.ContainerId = newContainerId;

        //        await SaveChangesAsync();
        //        return newContainerId;
        //    }
        //    else
        //    {
        //        throw new UnauthorizedAccessException($"The product wit id {productId} can't fit in the new container");
        //    }
        //}

        /// <summary>
        /// Gets a container by its ID with all details
        /// </summary>
        public async Task<ContainerInfo> GetContainerById(int containerId)
        {
            var container = await Containers.Where(c => c.Id == containerId).FirstOrDefaultAsync();

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
            };
        }

        /// <summary> 
        /// Gets a summary of products in a container including available space
        /// </summary>
        public Task<ProductSummary> GetContainerProductsSummary(int containerId, string agentCode)
        {
            var container = Containers
                .Include(p => p.Products)
                .Where(c => c.Id == containerId && c.Products.Any(p => !p.IsDeleted))
                .SingleOrDefault();


            if (container == null)
            {
                return null;
            }



            AuthorizationCheck(agentCode, container.AgentCode);


            var availableVolume = Container.MAX_VOLUME - container.TotalVolume;
            var utilizationPercentage = (container.TotalVolume / Container.MAX_VOLUME) * 100;

            return Task.FromResult(new ProductSummary
            {
                ContainerNumber = container.ContainerNumber,
                ProductCategoryName = container.ProductCategory.ToString(),
                AgentCode = container.AgentCode,
                TotalWeight = container.TotalWeight,
                TotalVolume = container.TotalVolume,
                AvailableVolume = availableVolume,
                VolumeUtilizationPercentage = utilizationPercentage,
                Count = Containers.Where(c => c.Id == containerId).SingleOrDefault().Products.Count()
            });
        }

        public async Task<List<SpecificProductSummary>> GetAllProductsDetails(int containerId, string agentCode)
        {
            var container = await Containers.Where(c => c.Id == containerId).FirstOrDefaultAsync();

            AuthorizationCheck(agentCode, container.AgentCode);

            var registeredProducts = Containers
                .Include(i => i.Products)
                .Where(c => c.Id == containerId)
                .SingleOrDefault()?.Products
                .Select
                (product => new SpecificProductSummary
                {
                    ProductCategoryName = product.CategoryCode.ToString(),
                    ProductName = product.ProductName,
                    Id = product.Id,
                    Weight = product.Weight,
                    Volume = product.Volume,
                    Notes = product.Notes,
                    IsDeleted = product.IsDeleted
                })
                .ToList();

            //foreach (var product in (registeredProducts))
            //{

            //    productList.Add(new SpecificProductSummary
            //    {
            //        ProductCategoryName = container.ProductCategory?.ToString(),
            //        ProductName = product.ProductName,
            //        Id = product.Id,
            //        Weight = product.Weight,
            //        Volume = product.Volume,
            //        Notes = product.Notes

            //    });
            //}

            return registeredProducts;
        }

        public BookingSummary GetBookingById(int bookingId)
        {

            var booking = Bookings
                .Include(i => i.Containers)
                .Where(c => c.Id == bookingId)
                .SingleOrDefault();
            var bookingContainers = booking
                .Containers
                .Select
                (container => new SpecificContainerSummary
                {
                    Id = container.Id,
                    ContainerNumber = container.ContainerNumber,
                    ProductCategory = container.ProductCategory,
                    TotalWeight = container.TotalWeight,
                    TotalVolume = container.TotalVolume
                })
                .ToList();

            if (booking == null)
            {
                return null;
            }

            return new BookingSummary
            {
                Id = booking.Id,
                BookingNumber = booking.BkNumber,

                CustomerInformations =
                    $"{booking.CustomerName} {booking.CustomerSurname} {booking.CustomerTaxCode} - {booking.CustomerCompany}",
                OriginAdress =
                    $"{booking.OriginNation}, {booking.OriginCity}, {booking.OriginAddress}, {booking.OriginPostalCode}",

                ConsigneeInformations =
                    $"{booking.ConsigneeName} {booking.ConsigneeSurname} {booking.ConsigneeTaxCode} - {booking.ConsigneeCompany}",
                DestinationAdress =
                    $"{booking.DestinationNation}, {booking.DestinationCity}, {booking.DestinationAddress}, {booking.DestinationPostalCode}",

                //If no containers, create empty list
                ContainerSummaries = bookingContainers ?? new(),


            };
        }

        /// <summary>
        /// Gets all available MSC category codes with descriptions fixx
        /// </summary>
        public List<CategoryInfo> GetAllCategoryDetails()
        {
            return Enum.GetValues<MscCategoryCode>()
                   .Select(code => new CategoryInfo
                   {
                       Code = (int)code,
                       Description = code.ToString()
                   }).ToList();
        }

        /// <summary>
        /// Calculates the average temperature for a container from all its telemetry readings
        /// </summary>
        public async Task<float?> GetAverageTemperature(int containerId, string agentCode)
        {
            var container = await Containers
                .Where(c => c.Id == containerId && c.Products.Any(p => !p.IsDeleted))
                .FirstOrDefaultAsync();

            AuthorizationCheck(agentCode, container.AgentCode);

            var averageTemp = await Containers
            .Include(c => c.Telemetries)
            .Where(c => c.Id == containerId)
            .Select(c => c.Telemetries.Average(t => t.Temperature))
            .FirstOrDefaultAsync();

            return averageTemp;
        }



        /// <summary>
        /// Gets all container IDs
        /// </summary>
        public async Task<List<int>> GetAllContainerIds()
        {
            return await Containers
                        .Select(c => c.Id)
            .ToListAsync();
        }

        // Helper Methods

        /// <summary>
        /// Gets the ideal temperature for each product category
        /// </summary>
        private double GetTemperatureSetpoint(MscCategoryCode category)
        {
            return category switch
            {
                MscCategoryCode.Pharmaceuticals => 5.0,// 2-8°C
                MscCategoryCode.HighlyPerishable => 1.0,  // -2 to +4°C
                MscCategoryCode.FreshProduce => 7.0, // 2-12°C
                MscCategoryCode.Frozen => -21.0,    // -18 to -25°C
                MscCategoryCode.TemperatureControlled => 18.0,    // 10-25°C
                _ => 5.0
            };
        }

        /// <summary>
        /// Gets the ideal humidity for each product category
        /// </summary>
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

        public async Task ChangeTemperatureAndHumidity(
            int containerId,
            string agentCode,
            double? newTemperatureSetpoint,
            double? newHumiditySetpoint)
        {
            // Find the container
            var container = await Containers
                .Where(c => c.Id == containerId && c.Products.Any(p => !p.IsDeleted))
                .FirstOrDefaultAsync();

            if (container == null)
            {
                throw new ArgumentException($"Container with ID {containerId} not found");
            }

            // Checks if the agency code inserted doesn't match the agency code of the container 
            AuthorizationCheck(agentCode, container.AgentCode);

            // Not negative number
            if (newHumiditySetpoint < 0)
            {
                throw new ArgumentException("Humidity can't accept negative values");
            }

            if (newHumiditySetpoint is null && newTemperatureSetpoint is null)
            {
                throw new ArgumentException("Insert at least one value!");
            }

            // Update product quantities

            if (newTemperatureSetpoint != null)
            {
                container.TemperatureSetpoint = newTemperatureSetpoint;
            }
            if (newHumiditySetpoint != null)
            {
                container.HumiditySetpoint = newHumiditySetpoint;
            }

            await SaveChangesAsync();
            //DatabaseSeeder.SeedDatabase(modelBuilder);
        }

        public List<string> GetContainerOutOfRangeByBookingId(int bookingId)
        {

            var containers = Containers.Include(c => c.Telemetries).Where(i => i.BookingId == bookingId);

            if (containers == null)
            {
                return null;
            }

            List<Container> containersOutOfRange = new();
            foreach (var container in containers)
            {
                var tempTollerance = MscCategoryCodeExtensions.GetTemperatureTolerance((MscCategoryCode)container.ProductCategory);
                foreach (var telemetry in container.Telemetries)
                {
                    if (telemetry.Temperature < (container.TemperatureSetpoint - tempTollerance) ||
                    telemetry.Temperature > (container.TemperatureSetpoint + tempTollerance))
                    {
                        containersOutOfRange.Add(container);
                    }

                }
            }
            List<string> txtOutOfRange = containersOutOfRange.Select(container => $"{container.Id} - {container.ContainerNumber}").ToList();

            return txtOutOfRange;
        }

        public List<string> GetTelemetriesOutOfRange(int containerId)
        {

            var containers = Containers.Include(c => c.Telemetries)
                .Where(c => c.Id == containerId);

            if (containers == null)
            {
                return null;
            }

            var container = Containers.Include(c => c.Telemetries)
                .Where(c => c.Id == containerId)
                .SingleOrDefault();

            var tempTollerance = MscCategoryCodeExtensions.GetTemperatureTolerance((MscCategoryCode)container.ProductCategory);
            var temperatureSetpoint = container.TemperatureSetpoint;

            var telemetriesOutOfRange = container.Telemetries
                .Where(t => IsOutOfRange(t, tempTollerance, temperatureSetpoint)).DistinctBy(d => d.DateRegistered);

            return telemetriesOutOfRange.Select(s => $"{container.Id} - {s.DateRegistered} - {s.Temperature} °C").ToList();
        }

        private bool IsOutOfRange(Telemetry telemetry, double tempTollerance, double? temperatureSetpoint)
        {
            return telemetry.Temperature < (temperatureSetpoint - tempTollerance) ||
                   telemetry.Temperature > (temperatureSetpoint + tempTollerance);
        }

        /// <summary>
        /// Remove some products
        /// </summary>
        public async Task<string> RemoveProduct(bool softOrHardDelete, string agentCode, int containerId, string productName, double weight, double volume)
        {
            var container = await Containers
            .Where(c => c.Id == containerId).FirstOrDefaultAsync();

            if (container == null)
            {
                throw new ArgumentException($"Container with ID {containerId} not found");
            }

            // Checks if the agency code inserted doesn't match the agency code of the container 
            AuthorizationCheck(agentCode, container.AgentCode);

            var products = await Products
                .Where(c =>
                    c.ContainerId == containerId &&
                    c.ProductName == productName &&
                    c.Weight == weight &&
                    c.Volume == volume)
                .FirstOrDefaultAsync();

            if (products == null || products.IsDeleted == true)
            {
                throw new Exception("Product not found or already deleted");
            }

            if (softOrHardDelete == false) // SOFT DELETE 
            {
                products.IsDeleted = true;

                container.TotalVolume = container.Products
                    .Where(product => !product.IsDeleted != false).Sum(s => s.Volume);

                container.TotalWeight = container.Products
                    .Where(product => product.IsDeleted != false).Sum(s => s.Weight);

            }
            else  // HARD DELETE == TRUE
            {
                container.TotalVolume -= volume;
                container.TotalWeight -= weight;

                Products.Remove(products);
            }


            await SaveChangesAsync();

            return productName;
        }

        public void AuthorizationCheck(string agencyCode, string DBAgencyCode)
        {
            if (agencyCode != DBAgencyCode)
            {
                throw new UnauthorizedAccessException($"This agency code : {agencyCode} doesn't have the authorization to perform this action!");
            }
        }
    }
}
