using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MongoDb;

namespace ProductApiTests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly MongoDbContainer _mongoContainer;

    public TestWebApplicationFactory()
    {
        _mongoContainer = new MongoDbBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
        Environment.SetEnvironmentVariable("MONGODBSETTINGS__CONNECTIONSTRING", _mongoContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("MONGODBSETTINGS__PRODUCTTESTDB", "ProductTestDb");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        IConfiguration conf = new ConfigurationBuilder()
            .AddJsonFile("testappsettings.json")
            .AddEnvironmentVariables()
            .Build();

        builder.UseEnvironment("Development");
        builder.UseConfiguration(conf);

        base.ConfigureWebHost(builder);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _mongoContainer.StopAsync();
    }
}
