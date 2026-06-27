using Microsoft.AspNetCore.Mvc;
using ReeferSentinel.Monolith.Data;
using ReeferSentinel.Monolith.Models;

namespace ReeferSentinel.Monolith.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDatabase _database;

        public ProductController(AppDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Adds products to an existing container
        /// </summary>
        /// <param name="containerId">ID of the container</param>
        /// <param name="productWeight">Total weight of products in kg</param>
        /// <param name="productVolume">Total volume of products in cubic meters</param>
        /// <param name="productCategory">Category of the products</param>
        [HttpPut(nameof(AddProductInContainer))]
        public async Task<IActionResult> AddProductInContainer(
            int containerId,
            MscCategoryCode categoryCode,
            string productName,
            string notes,
            double productWeight,
            double productVolume,
            string AgentCode)
        {

            if (containerId == 0 || productWeight == 0 || productVolume == 0 || productName == "N/D")
            {
                throw new ArgumentException("One or more values are set to 0 or to a negative value.");
            }

            var productID = await _database.AddProductsToContainer(
                containerId,
                productName,
                notes,
                categoryCode,
                productWeight,
                productVolume,
                AgentCode
                );

            return Ok(productID);
        }

        /// <summary>
        /// Adds products to an existing container
        /// </summary>
        /// <param name="containerId">ID of the container</param>
        /// <param name="productWeight">Total weight of products in kg</param>
        /// <param name="productVolume">Total volume of products in cubic meters</param>
        /// <param name="productCategory">Category of the products</param>
        //[HttpPut(nameof(ChangeProductLocation))]
        //public async Task<IActionResult> ChangeProductLocation(
        //    int containerId,
        //    int newContainerId,
        //    int productId,
        //    string AgentCode)
        //{

        //    if (containerId == 0 || newContainerId == 0 || productId == 0 || AgentCode == null)
        //    {
        //        throw new ArgumentException("One or more values are set to 0 or to a negative value.");
        //    }

        //    var productID = await _database.ChangeProductLocation( 
        //        containerId,
        //        newContainerId,
        //        productId,
        //        AgentCode
        //        );

        //    return Ok(productID);
        //}


        [HttpGet(nameof(GetAllProductsDetails))]
        public async Task<IActionResult> GetAllProductsDetails([FromQuery] int containerId, string AgentCode)
        {
            var productList = await _database.GetAllProductsDetails(containerId, AgentCode);

            if (productList == null)
            {
                return NotFound($"Container with ID {containerId} doesn't have any products");
            }

            return Ok(productList);
        }

        /// <summary>
        /// Remove some products
        /// </summary>
        /// <param name="containerId">ID of the container</param>
        [HttpDelete(nameof(DeleteProduct))]
        public async Task<IActionResult> DeleteProduct([FromQuery]
            bool hardDelete,
            string AgentCode,
            int containerID,
            string productName,
            double weight,
            double volume)
        {
            var msg = await _database.RemoveProduct(hardDelete, AgentCode, containerID, productName, weight, volume);

            if (msg == null)
            {
                return NotFound($"Product {productName} in container {containerID} not found");
            }

            return Ok(msg);
        }


    }
}
