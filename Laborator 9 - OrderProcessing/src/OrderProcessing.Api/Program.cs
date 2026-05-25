using OrderProcessing.Api.Endpoints;
using OrderProcessing.Api.Services;
using OrderProcessing.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

builder.Services.AddSingleton<IOrderValidationHandler>(_ =>
{
    var stock = new StockValidationHandler();
    var price = new PriceValidationHandler();
    var fraud = new FraudDetectionHandler();
    var age   = new AgeVerificationHandler();
    stock.SetNext(price).SetNext(fraud).SetNext(age);
    return stock;
});

builder.Services.AddSingleton<OrderService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "OrderProcessing API", Version = "v1",
        Description = "Lab 4 — Chain of Responsibility + State Pattern" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderProcessing v1");
    c.RoutePrefix = "swagger";
});

app.UseStaticFiles();                         
app.MapOrderEndpoints();
app.MapFallbackToFile("index.html");         

app.Run("http://localhost:5000");
