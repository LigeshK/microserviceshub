using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
    //Marten has a feature to make another column as Identity field than Id
    //to set this added the below line of code - Use UserName property as Identity field
    options.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

//Add dependency injection for BasketRepository
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
//Need to cache CachebasketRepository from IBasketRepository - But we cant have 2 registrations as the second with override the first
//One many is manual injection via code. Better way is the use Scrutor library - which registers (decorates) already registered service

//Install Scrutor via nuget and use Decorate method
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

//Register IDistributedCache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

//Add dependency injection for ExceptionHandler
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

//Register healthchecks for Postgres DB and Redis cache
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();
//Include exception in request pipeline
app.UseExceptionHandler(options => { });

//Implement health checks - Install nuget health.UI.client - to get health checkwriter to return response as JSON format
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
