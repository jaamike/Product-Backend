using ProductAPI.Models;      // Imports the Product model class
using ProductAPI.Repositories; // Imports the IProductRepository interface
using System;                 // Imports basic system classes like Guid
using System.Collections.Generic; // Imports collection types like IEnumerable
using System.Threading.Tasks; // Imports types for asynchronous programming

namespace ProductAPI.Services
{
    // Service class for handling business logic related to products
    public class ProductService
    {
        private readonly IProductRepository _repository; // Repository interface for product data operations

        // Constructor to initialize the service with a repository instance
        public ProductService(IProductRepository repository)
        {
            _repository = repository; // Sets the repository instance for data operations
        }

        // Asynchronously retrieves all products using the repository
        public Task<IEnumerable<Product>> GetAllProductsAsync() => _repository.GetAllProductsAsync();

        // Asynchronously retrieves a single product by its ID using the repository
        public Task<Product> GetProductByIdAsync(Guid id) => _repository.GetProductByIdAsync(id);

        // Asynchronously creates a new product using the repository
        public Task CreateProductAsync(Product product) => _repository.CreateProductAsync(product);

        // Asynchronously updates an existing product using the repository
        public Task UpdateProductAsync(Product product) => _repository.UpdateProductAsync(product);

        // Asynchronously deletes a product by its ID using the repository
        public Task DeleteProductAsync(Guid id) => _repository.DeleteProductAsync(id);
    }
}
