using ProductApi.Models;
using ProductApi.Repositories;

namespace ProductApi.Services;

public class ProductService: IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IEnumerable<Product> GetProducts()
    {
        return _productRepository.GetProducts();
    }

    public Product GetProductById(string id)
    {
        return _productRepository.GetProductById(id);
    }

    public void CreateProduct(Product product)
    {
        _productRepository.CreateProduct(product);
    }

    public void UpdateProduct(string id, Product product)
    {
        _productRepository.UpdateProduct(id, product);
    }

    public void DeleteProduct(string id)
    {
        _productRepository.DeleteProduct(id);
    }
}
