using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Features.GettingPublication;
using SocialNetwork.Application.Services.PublicationService;
using SocialNetwork.Application.Services.UserServices;
using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Domain.Jobs;
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
builder.Services.AddScoped<IPublicationService, PublicationService>();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    // Just use the name of your job that you created in the Jobs folder.
    q.AddJob<AddRandomFriendsJob>(AddRandomFriendsJob.Key);

    q.AddTrigger(opts => opts
        .ForJob(AddRandomFriendsJob.Key)
        .WithIdentity("AddRandomFriendsJob-startTrigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(1)
            .RepeatForever())
        .StartNow()
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapPost("api/addUser", async (UserInputDto inputUser,IUserService userService) =>
{
    var newUser = await userService.AddUser(inputUser);
    return Results.Created($"api/addUser/{newUser.Id}", newUser);
});

app.MapPost("api/addPublications", async (PublicationInputDto publicationInputDto, IUserService userService) =>
{
    var newPublication = await userService.AddPublication(publicationInputDto);

    return Results.Created($"api/addPublications/{newPublication.Id}", newPublication);
});

app.MapGet("api/getPublication", async (IMediator mediator)=> await mediator.Send(new GetPublication()));

app.Run();