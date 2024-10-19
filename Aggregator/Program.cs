using System.Text.Json;
using Consul;
using Microsoft.AspNetCore.Mvc.ViewEngines;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IUserService, UserServiceClient>();
builder.Services.AddHttpClient<IPublicationService, PublicationServiceClient>();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri("http://localhost:8500");
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/users-publications", async (HttpContext httpContext, IUserService userService, IPublicationService publicationService) =>
{
    var users = userService.GetUsers();
    var publications = publicationService.GetPublictions();

    var usersWithPublications = users.Select(x => new
    {
        x.Id,
        x.Name,
        Publications = publications.Where(pub => pub.userId == x.Id.ToString())
    });

    httpContext.Response.ContentType = "application/json";
    await JsonSerializer.SerializeAsync(httpContext.Response.Body, usersWithPublications);
});

app.Run();

public interface IUserService
{
    List<User> GetUsers();
}

public interface IPublicationService
{
    List<Publiction> GetPublictions();
}

public class UserServiceClient : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly IConsulClient _consulClient;

    public UserServiceClient(HttpClient httpClient, IConsulClient consulClient)
    {
        _httpClient = httpClient;
        _consulClient = consulClient;
    }

    public List<User> GetUsers()
    {
        var services = _consulClient.Agent.Services().Result.Response;
        var user = services.Values.FirstOrDefault(s => s.Service.Equals("user"));
        var response = _httpClient.GetStringAsync($"http://{user.Address}:{user.Port}/users").Result;
        var users = JsonSerializer.Deserialize<List<User>>(response);
        return users;
    }
}

public class PublicationServiceClient : IPublicationService
{
    private readonly HttpClient _httpClient;
    private readonly IConsulClient _consulClient;
    public PublicationServiceClient(HttpClient httpClient, IConsulClient consulClient)
    {
        _httpClient = httpClient;
        _consulClient = consulClient;
    }
    public List<Publiction> GetPublictions()
    {
        var services = _consulClient.Agent.Services().Result.Response;
        var publication = services.Values.FirstOrDefault(s => s.Service.Equals("socialNetwork"));
        var response = _httpClient.GetStringAsync($"http://{publication.Address}:{publication.Port}/api/GetPublicationsWithoutAuth").Result;
        var publications = JsonSerializer.Deserialize<List<Publiction>>(response);
        return publications;
    }
}

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; }
}

public class Publiction
{
    public string id { get; set; }
    public string userId { get; set; }
    public string mediaContent { get; set; }
    public string textContent { get; set; }

}