using ProductApi.GraphQL;
using ProductApi.Repositories;
using ProductApi.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMongoClient>(provider =>
{
    var connectionString = builder.Configuration.GetSection("MongoDBSettings:ConnectionString").Value;
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(provider =>
{
    var client = provider.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration.GetSection("MongoDBSettings:DatabaseName").Value;
    return client.GetDatabase(databaseName);
});

builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddGraphQLServer()
    .AddQueryType<ProductQueryType>()
    .AddType<ProductType>()
    .AddMutationType<ProductMutationType>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); 
app.MapGraphQL("/graphql");

app.MapControllers();

app.Run();


public partial class Program { }