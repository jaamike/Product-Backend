using Microsoft.AspNetCore.Mvc; // Imports ASP.NET Core MVC features
using ProductAPI.Models;         // Imports the Product model class
using ProductAPI.Services;       // Imports the ProductService class
using System;                    // Imports basic .NET types
using System.Collections.Generic; // Imports collection types like List and IEnumerable
using System.Threading.Tasks;    // Imports types for asynchronous programming

namespace ProductAPI.Controllers
{
    // Marks the class as an API controller with automatic model validation
    [ApiController]
    // Sets the base route for all actions in this controller to "api/products"
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Private field to hold the service instance
        private readonly ProductService _service;

        // Constructor to inject the ProductService dependency
        public ProductsController(ProductService service)
        {
            _service = service; // Initializes the service field
        }

        // Action method to handle GET requests to "api/products"
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            // Calls the service to retrieve all products asynchronously
            return await _service.GetAllProductsAsync();
        }

        // Action method to handle GET requests to "api/products/{id}"
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(Guid id)
        {
            // Calls the service to retrieve a product by its ID asynchronously
            var product = await _service.GetProductByIdAsync(id);

            // If the product is not found, return a 404 Not Found response
            if (product == null)
            {
                return NotFound();
            }

            // Return the found product
            return product;
        }

        // Action method to handle POST requests to "api/products"
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            // Calls the service to create a new product asynchronously
            await _service.CreateProductAsync(product);

            // Returns a 201 Created response with the location of the new resource
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        // Action method to handle PUT requests to "api/products/{id}"
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Product updatedProduct)
        {
            // Checks if the ID in the route matches the ID in the updated product
            if (id != updatedProduct.Id)
            {
                // Returns a 400 Bad Request response if IDs do not match
                return BadRequest();
            }

            // Calls the service to retrieve the existing product by ID
            var product = await _service.GetProductByIdAsync(id);

            // If the product is not found, return a 404 Not Found response
            if (product == null)
            {
                return NotFound();
            }

            // Calls the service to update the product asynchronously
            await _service.UpdateProductAsync(updatedProduct);

            // Returns a 204 No Content response indicating successful update
            return NoContent();
        }

        // Action method to handle DELETE requests to "api/products/{id}"
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Calls the service to retrieve the product by ID
            var product = await _service.GetProductByIdAsync(id);

            // If the product is not found, return a 404 Not Found response
            if (product == null)
            {
                return NotFound();
            }

            // Calls the service to delete the product asynchronously
            await _service.DeleteProductAsync(id);

            // Returns a 204 No Content response indicating successful deletion
            return NoContent();
        }
    }
}
