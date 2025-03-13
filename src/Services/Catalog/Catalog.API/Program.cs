var builder = WebApplication.CreateBuilder(args);

//Add Services

//Register Mediatr and add all pipeline behaviors into it.
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);

    //Add validator behavior as pipeline behavior
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

//Register Global custom exception handler 
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapCarter();
//Add exception pipeline - Make application use thie exception handler pipeline
//Empty options indicates that we rely on custom config handler - catches all unhandled exceptions and typically returns the generic error
app.UseExceptionHandler(options => { });

app.Run();
