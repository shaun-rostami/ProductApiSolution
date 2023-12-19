using System.Text;
using System.Text.Json;
using Bogus;
using ProductApi.Models;

namespace ProductApiTests;

public class GraphQLIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public GraphQLIntegrationTests(TestWebApplicationFactory<Program> factory)

    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task CanAddNewProductAndGetItsDetails()
    {
        var fakeObject = new Faker<Product>()
            .RuleFor(p=>p.Name, x=>x.Random.Word())
            .RuleFor(p=>p.Id, x=>x.Random.Word())
            .RuleFor(p=>p.Price, x=>x.Random.Decimal(min: 5m, max:100m)).Generate();

        var id = fakeObject.Id;
        var name = fakeObject.Name;
        var price = fakeObject.Price;

        var url = _httpClient.BaseAddress + "graphql";

        // Create a new product
        var mutationBody = $@"mutation{{addProduct(price:{price},name:""{name}"",id:""{id}""){{id,name,price}}}}";

        var mutationObject = new
        {
            query = mutationBody
        };

        var mutationRequest = BuildRequest(mutationObject, url);

        using var mutationResponse = await _httpClient.SendAsync(mutationRequest);

        Assert.True(mutationResponse.IsSuccessStatusCode);

        // Get the details of the new product
        var queryBody = $@"query{{productbyid(id:""{id}""){{id,name,price}}}}";

        var queryObject = new { query = queryBody };

        var queryRequest = BuildRequest(queryObject, url);

        using var queryResponse = await _httpClient.SendAsync(queryRequest);

        Assert.True(queryResponse.IsSuccessStatusCode);

        if (queryResponse.IsSuccessStatusCode)
        {
            var content = await queryResponse.Content.ReadAsStringAsync();
            var newProduct = JsonSerializer.Deserialize<ApiResponseContainer>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.Equal(id, newProduct?.Data?.ProductById?.Id);
            Assert.Equal(name, newProduct?.Data?.ProductById?.Name);
            Assert.Equal(price, newProduct?.Data?.ProductById?.Price);
        }
    }

    private HttpRequestMessage BuildRequest(object obj, string url)
    {
        var content = new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = content
        };

        return request;
    }
}

public class ApiResponseContainer
{
    public DataContainer Data { get; set; }
}

public class DataContainer
{
    public Product ProductById { get; set; }
}
