using ProductAPI.Models;        // Imports the Product model class
using System.Collections.Generic; // Imports collection types like List and IEnumerable
using System.Threading.Tasks;    // Imports types for asynchronous programming

namespace ProductAPI.Repositories
{
    // Defines the contract for a product repository with asynchronous operations
    public interface IProductRepository
    {
        // Asynchronously retrieves all products from the repository
        Task<IEnumerable<Product>> GetAllProductsAsync();

        // Asynchronously retrieves a single product by its ID
        Task<Product> GetProductByIdAsync(Guid id);

        // Asynchronously creates a new product in the repository
        Task CreateProductAsync(Product product);

        // Asynchronously updates an existing product in the repository
        Task UpdateProductAsync(Product product);

        // Asynchronously deletes a product by its ID from the repository
        Task DeleteProductAsync(Guid id);
    }
}
