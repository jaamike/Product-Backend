using MongoDB.Driver; // Provides classes and methods to interact with MongoDB databases.
using Moq; // A mocking framework for creating mock objects to simulate and verify interactions in unit tests.
using NUnit.Framework; // A unit testing framework for writing and running tests in C#.
using ProductAPI.Models; // Contains the models used in the ProductAPI project, such as `Product`.
using ProductAPI.Repositories; // Contains the repository classes that handle data access, such as `ProductRepository`.
using System; // Provides fundamental types and utilities, including `Guid` and `Console`.
using System.Collections.Generic; // Contains types for handling collections, such as `List<T>`.
using System.Linq; // Provides LINQ methods for querying and manipulating collections.
using System.Threading; // Supports asynchronous programming and thread management, including `CancellationToken`.
using System.Threading.Tasks; // Provides support for asynchronous operations, including `Task` and `Task<T>`.
using Microsoft.Extensions.Logging; // Provides logging abstractions and interfaces, such as `ILogger`, for logging in .NET applications.

namespace ProductTests
{
    // Test class for the ProductRepository class.
    public class ProductRepositoryTests
    {
        private Mock<IMongoCollection<Product>> _mockProductCollection; // Mock for the MongoDB collection of products.
        private Mock<ILogger<ProductRepository>> _mockLogger; // Mock for the logger used by the ProductRepository.
        private Mock<IAsyncCursor<Product>> _mockCursor; // Mock for the MongoDB cursor used to simulate database queries.
        private ProductRepository _repository; // Instance of the ProductRepository class to be tested.

        [SetUp]
        public void Setup()
        {
            // ARRANGE
            _mockProductCollection = new Mock<IMongoCollection<Product>>(); // Create a mock of the MongoDB product collection.
            _mockLogger = new Mock<ILogger<ProductRepository>>(); // Create a mock of the logger.

            _mockCursor = new Mock<IAsyncCursor<Product>>(); // Create a mock of the MongoDB cursor.
            _mockCursor.Setup(_ => _.Current).Returns(new List<Product>().AsQueryable()); // Setup the Current property to return an empty list initially.
            _mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>())) // Setup MoveNext to simulate iteration over the cursor.
                .Returns(true) // First call returns true (indicating there are more documents).
                .Returns(false); // Second call returns false (indicating no more documents).
            _mockCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>())) // Setup asynchronous iteration over the cursor.
                .Returns(Task.FromResult(true)) // First call returns a completed task with true.
                .Returns(Task.FromResult(false)); // Second call returns a completed task with false.

            var mockDatabase = new Mock<IMongoDatabase>(); // Create a mock of the MongoDB database.
            mockDatabase.Setup(db => db.GetCollection<Product>(It.IsAny<string>(), null))
                        .Returns(_mockProductCollection.Object); // Setup GetCollection to return the mock product collection.

            _repository = new ProductRepository(mockDatabase.Object, _mockLogger.Object); // Initialize the repository with mocked dependencies.
        }

        [Test]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // ARRANGE
            Console.WriteLine("Setting up test for GetAllProductsAsync.");
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1", Price = 100 },
                new Product { Id = Guid.NewGuid(), Name = "Product 2", Price = 200 }
            };

            _mockProductCollection
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object); // Setup FindAsync to return the mock cursor.
            _mockCursor.SetupSequence(_ => _.Current).Returns(products); // Setup Current to return a list of products.

            // ACT
            Console.WriteLine("Executing GetAllProductsAsync.");
            var result = await _repository.GetAllProductsAsync(); // Call the method under test.

            // ASSERT
            Console.WriteLine($"Result count: {result.Count()}");
            Assert.AreEqual(2, result.Count()); // Verify the number of products returned matches the expected count.
        }

        [Test]
        public async Task GetProductByIdAsync_ShouldReturnProduct()
        {
            // ARRANGE
            Console.WriteLine("Setting up test for GetProductByIdAsync.");
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product 1", Price = 100 };

            _mockProductCollection
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object); // Setup FindAsync to return the mock cursor.
            _mockCursor.SetupSequence(_ => _.Current).Returns(new List<Product> { product }); // Setup Current to return a single product.

            // ACT
            Console.WriteLine($"Executing GetProductByIdAsync for Product ID: {productId}");
            var result = await _repository.GetProductByIdAsync(productId); // Call the method under test.

            // ASSERT
            Console.WriteLine($"Product fetched: {result?.Name}");
            Assert.NotNull(result); // Verify the result is not null.
            Assert.AreEqual(productId, result.Id); // Verify the product ID matches the expected value.
        }

        [Test]
        public async Task CreateProductAsync_ShouldCallInsertOneAsync()
        {
            // ARRANGE
            Console.WriteLine("Setting up test for CreateProductAsync.");
            var product = new Product { Id = Guid.NewGuid(), Name = "New Product", Price = 100 };

            // ACT
            Console.WriteLine($"Creating product with ID: {product.Id}");
            await _repository.CreateProductAsync(product); // Call the method under test.

            // ASSERT
            Console.WriteLine("Verifying InsertOneAsync was called.");
            _mockProductCollection.Verify(x => x.InsertOneAsync(product, null, default), Times.Once); // Verify InsertOneAsync was called exactly once.
        }

        [Test]
        public async Task UpdateProductAsync_ShouldCallReplaceOneAsync()
        {
            // ARRANGE
            Console.WriteLine("Setting up test for UpdateProductAsync.");
            var product = new Product { Id = Guid.NewGuid(), Name = "Updated Product", Price = 150 };

            // ACT
            Console.WriteLine($"Updating product with ID: {product.Id}");
            await _repository.UpdateProductAsync(product); // Call the method under test.

            // ASSERT
            Console.WriteLine("Verifying ReplaceOneAsync was called.");
            _mockProductCollection.Verify(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<Product>>(), product, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()), Times.Once); // Verify ReplaceOneAsync was called exactly once.
        }

        [Test]
        public async Task DeleteProductAsync_ShouldCallDeleteOneAsync()
        {
            // ARRANGE
            Console.WriteLine("Setting up test for DeleteProductAsync.");
            var productId = Guid.NewGuid();

            // ACT
            Console.WriteLine($"Deleting product with ID: {productId}");
            await _repository.DeleteProductAsync(productId); // Call the method under test.

            // ASSERT
            Console.WriteLine("Verifying DeleteOneAsync was called.");
            _mockProductCollection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<CancellationToken>()), Times.Once); // Verify DeleteOneAsync was called exactly once.
        }
    }
}
