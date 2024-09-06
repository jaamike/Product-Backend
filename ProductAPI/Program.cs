using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductAPI.Repositories;
using ProductAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MongoDB services to the container
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
    return client.GetDatabase("ProductTest");  // Replace with your actual database name
});

// Add application services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

// Add controllers and configure Swagger/OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply CORS policy
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();
