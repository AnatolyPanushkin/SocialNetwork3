using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Services.UserServices;
using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SocialNetworkContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=SocialNetwork;Username=postgres;Password=1234",
        b => b.MigrationsAssembly("SocialNetwork"));
});

//dotnet ef migrations add init
//dotnet ef database update

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapPost("api/adduser", async (UserInputDto inputUser,IUserService userService) =>
{
    var newUser = await userService.AddUser(inputUser.FirstName, inputUser.LastName, inputUser.Birthday);
    return Results.Created($"api/adduser/{newUser.Id}", newUser);
});

app.MapPost("api/publications", async (PublicationInputDto publication, IUserService userService) =>
{
    var newPublication = await userService.AddPublication(publication.TextContent, publication.MediaContent, publication.UserGuidId);

    return Results.Created($"api/publications/{newPublication.Id}",newPublication);
});

app.MapPost("api/getpublications", async (Guid curUserId, Guid userId, IUserService userService) =>
{
    var listPublication = await userService.GetListPublications(curUserId, userId);

    return Results.Created($"api/getpublications/", listPublication);
});

app.Run();