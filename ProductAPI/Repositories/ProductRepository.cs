using MongoDB.Driver;             // Imports MongoDB driver for database operations
using ProductAPI.Models;          // Imports the Product model class
using System;                     // Imports basic system classes like Exception and Guid
using System.Collections.Generic; // Imports collection types like List and IEnumerable
using System.Threading.Tasks;    // Imports types for asynchronous programming
using Microsoft.Extensions.Logging; // Imports logging functionality

namespace ProductAPI.Repositories
{
    // Implementation of the IProductRepository interface using MongoDB
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products; // MongoDB collection for products
        private readonly ILogger<ProductRepository> _logger;  // Logger for the repository

        // Constructor to initialize the repository with MongoDB database and logger
        public ProductRepository(IMongoDatabase database, ILogger<ProductRepository> logger)
        {
            _products = database.GetCollection<Product>("Products"); // Gets the MongoDB collection named "Products"
            _logger = logger; // Initializes the logger
        }

        // Asynchronously retrieves all products from the MongoDB collection
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all products."); // Logs information about the operation
                return await _products.Find(product => true).ToListAsync(); // Fetches all products and converts to list
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products."); // Logs any errors that occur
                throw; // Rethrows the exception to be handled by the caller
            }
        }

        // Asynchronously retrieves a single product by its ID
        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching product with ID: {ProductId}", id); // Logs information about the operation
                return await _products.Find(product => product.Id == id).FirstOrDefaultAsync(); // Fetches the product by ID
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product with ID: {ProductId}", id); // Logs any errors that occur
                throw; // Rethrows the exception to be handled by the caller
            }
        }

        // Asynchronously creates a new product in the MongoDB collection
        public async Task CreateProductAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Creating product with ID: {ProductId}", product.Id); // Logs information about the operation
                await _products.InsertOneAsync(product); // Inserts the product into the collection
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product with ID: {ProductId}", product.Id); // Logs any errors that occur
                throw; // Rethrows the exception to be handled by the caller
            }
        }

        // Asynchronously updates an existing product in the MongoDB collection
        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", product.Id); // Logs information about the operation
                await _products.ReplaceOneAsync(p => p.Id == product.Id, product); // Replaces the existing product with the updated one
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", product.Id); // Logs any errors that occur
                throw; // Rethrows the exception to be handled by the caller
            }
        }

        // Asynchronously deletes a product by its ID from the MongoDB collection
        public async Task DeleteProductAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id); // Logs information about the operation
                await _products.DeleteOneAsync(product => product.Id == id); // Deletes the product by ID
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id); // Logs any errors that occur
                throw; // Rethrows the exception to be handled by the caller
            }
        }
    }
}
