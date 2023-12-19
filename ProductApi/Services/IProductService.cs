using ProductApi.Models;

namespace ProductApi.Services;

public interface IProductService
{
    IEnumerable<Product> GetProducts();
    Product GetProductById(string id);
    void CreateProduct(Product product);
    void UpdateProduct(string id, Product product);
    void DeleteProduct(string id);
}
