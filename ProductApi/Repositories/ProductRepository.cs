using MongoDB.Driver;
using ProductApi.Models;

namespace ProductApi.Repositories;

public class ProductRepository: IProductRepository
{
    private readonly IMongoCollection<Product> _collection;

    public ProductRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Product>("Products");
    }

    public IEnumerable<Product> GetProducts()
    {
        return _collection.Find(p => true).ToList();
    }

    public Product GetProductById(string id)
    {
        return _collection.Find(p => p.Id == id).FirstOrDefault();
    }

    public void CreateProduct(Product product)
    {
        _collection.InsertOne(product);
    }

    public void UpdateProduct(string id, Product product)
    {
        _collection.ReplaceOne(p => p.Id == id, product);
    }

    public void DeleteProduct(string id)
    {
        _collection.DeleteOne(p => p.Id == id);
    }
}
