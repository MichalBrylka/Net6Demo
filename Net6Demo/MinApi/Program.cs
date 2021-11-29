/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/hello", () => "Hello, World!");

app.Run();*/


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IHelloService, HelloService>();

var app = builder.Build();

app.MapGet("/hello", (HttpContext context, IHelloService helloService) => helloService.SayHello(context.Request.Query["name"].ToString()));

app.Run();


public interface IHelloService
{
    string SayHello(string name);
}

public class HelloService : IHelloService
{
    public string SayHello(string name) => $"Hello {name}";
}