var builder = WebApplication.CreateBuilder(args);

//Add Services
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
