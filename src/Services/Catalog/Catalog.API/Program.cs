
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

//Add Services

//Register Mediatr and add all pipeline behaviors into it.
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);

    //Add validator behavior as pipeline behavior
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

//Seed data - first time in Development environment
if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

//Register Global custom exception handler 
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

//Register healthcheck for Postgres database from catalog api
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

app.MapCarter();
//Add exception pipeline - Make application use thie exception handler pipeline
//Empty options indicates that we rely on custom config handler - catches all unhandled exceptions and typically returns the generic error
app.UseExceptionHandler(options => { });

//add healthcheck with UI
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
app.Run();
