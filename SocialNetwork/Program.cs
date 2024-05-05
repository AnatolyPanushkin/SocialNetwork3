using System.Reflection;
using Confluent.Kafka;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Features.GettingPublication;
using SocialNetwork.Application.Services.PublicationService;
using SocialNetwork.Application.Services.UserServices;
using SocialNetwork.Domain.Events.AddNewUser;
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

#region Jobs
//Add random friend job
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
//Add email approve consumer job
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    // Just use the name of your job that you created in the Jobs folder.
    q.AddJob<ApprovedEmailConsumer>(ApprovedEmailConsumer.Key);
    q.AddTrigger(opts => opts
        .ForJob(ApprovedEmailConsumer.Key)
        .WithIdentity("AproveEmailConsumer-startTrigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(1)
            .RepeatForever())
        .StartNow()
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
#endregion

#region Kafka
//-------------Add Kafka--------------------//
var producerConfig = new ProducerConfig
{
    BootstrapServers = $"localhost:29092",
    ClientId = "addNewUser"
};

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = $"localhost:29092",
    GroupId = "addNewUser-consumer",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

builder.Services.AddSingleton(new ProducerBuilder<string, string>(producerConfig).Build());
builder.Services.AddSingleton(new ConsumerBuilder<string, string>(consumerConfig).Build());
//------------------------------------------//
#endregion


//------------AddApproveEmailConsumer------------//
//builder.Services.AddHostedService<ApprovedEmailConsumer>();
//builder.Services.AddHostedService<ApproveEmailScopedServiceHostedService>();
//builder.Services.AddScoped<IScopedApprovedEmailService, ScopedApprovedEmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapPost("api/addUser", async (UserInputWithEmailDto userDto, IMediator mediator) =>
{
    await mediator.Send(new AddUserCommand(userDto));
});

app.MapPost("api/reportUser", async (IMediator mediator) =>
{
    
});

app.MapPost("api/addPublications", async (PublicationInputDto publicationInputDto, IUserService userService) =>
{
    var newPublication = await userService.AddPublication(publicationInputDto);

    return Results.Created($"api/addPublications/{newPublication.Id}", newPublication);
});

app.MapGet("api/getPublication", async (IMediator mediator)=> await mediator.Send(new GetPublication()));

app.Run();