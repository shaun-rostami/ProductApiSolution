using MongoDB.Bson;
using ProductApi.Models;
using ProductApi.Services;

namespace ProductApi.GraphQL;

public class ProductQueryType : ObjectType
{
    private readonly IProductService _productService;

    public ProductQueryType(IProductService productService)
    {
        _productService = productService;
    }

    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("products")
            .Type<ListType<ProductType>>()
            .Resolve(ctx =>
                _productService.GetProducts());


        descriptor.Field("productbyid")
            .Argument("id", arg => arg.Type<NonNullType<StringType>>())
            .Type<ProductType>()
            .Resolve(ctx =>
            {
                var id = ctx.ArgumentValue<string>("id");
                return _productService.GetProductById(id);
            });
    }
}

public class ProductMutationType : ObjectType
{
    private readonly IProductService _productService;

    public ProductMutationType(IProductService productService)
    {
        _productService = productService;
    }

    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("addProduct")
            .Argument("id", arg => arg.Type<NonNullType<StringType>>())
            .Argument("name", arg => arg.Type<NonNullType<StringType>>())
            .Argument("price", arg => arg.Type<NonNullType<DecimalType>>())
            .Type<ProductType>()
            .Resolve(ctx =>
            {
                var id = ctx.ArgumentValue<string>("id");
                var name = ctx.ArgumentValue<string>("name");
                var price = ctx.ArgumentValue<decimal>("price");
                var newProduct = new Product { Id = id, Name = name, Price = price };

                _productService.CreateProduct(newProduct);

                return newProduct;
            });
    }
}

public class ProductType : ObjectType<Product>
{
}
