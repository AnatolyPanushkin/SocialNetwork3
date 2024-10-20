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
using Newtonsoft.Json;
using Consul;
using Microsoft.Extensions.Options;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SocialNetworkContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=SocialNetwork;Username=postgres;Password=1234",
        b => b.MigrationsAssembly("SocialNetwork"));
});

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri("http://localhost:8500");
}));
builder.Services.Configure<ServiceDiscoveryConfig>(builder.Configuration.GetSection("ServiceDiscoveryConfig"));

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

app.UseConsul();

app.MapGet("/", () => "Hello World!");

#region User
app.MapPost("api/addUser", async (UserInputWithEmailDto userDto, IMediator mediator) =>
{
    await mediator.Send(new AddUserCommand(userDto));
});

app.MapPost("api/addUserWithoutApprove", async (UserInputWithEmailDto userDto, IMediator mediator) =>
{
    var createdUserId = await mediator.Send(new AddUserWithoutApproveCommand(userDto));
    return Results.Created($"api/addUserWithoutApprove/", createdUserId);
});

app.MapPost("api/reportUser", async (IMediator mediator) =>
{
    
});
#endregion

app.MapPost("api/addPublications", async (PublicationInputDto publicationInputDto, IUserService userService) =>
{
    var result = await userService.AddPublication(publicationInputDto);

    return TypedResults.Ok(result);
});

app.MapGet("api/getPublication", async (string userId, string ownerId, IMediator mediator) => 
{
    var result = await mediator.Send(new GetPublicationQuery(userId, ownerId));

    return TypedResults.Ok(result);
});

app.MapGet("api/GetPublicationByUserId", async (string userId, IMediator mediator) => 
{
    var result = await mediator.Send(new GetPublicationByUserIdQuery(userId));

    return TypedResults.Ok(result);
});

app.MapGet("api/GetPublicationsWithoutAuth", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetPublicationWithoutAuthQuery());

    return TypedResults.Ok(result);
});

app.Run();

public static class ConsulBuilderExtensions
{
    public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
    {

        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
        var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();

        var settings = app.ApplicationServices.GetRequiredService<IOptions<ServiceDiscoveryConfig>>();

        var serviceName = settings.Value.NameOfService;
        var serviceId = settings.Value.IdOfService;
        var uri = new Uri($"http://{settings.Value.Host}:{settings.Value.Port}");

        var registration = new AgentServiceRegistration()
        {
            ID = serviceId,
            Name = serviceName,
            Address = $"{settings.Value.Host}",
            Port = uri.Port,
            Tags = new[] { $"urlprefix-/{settings.Value.IdOfService}" }
        };

        var result = consulClient.Agent.ServiceDeregister(registration.ID).Result;
        result = consulClient.Agent.ServiceRegister(registration).Result;

        lifetime.ApplicationStopping.Register(() =>
        {
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        });

        return app;
    }
}

public record ServiceDiscoveryConfig
{
    public string NameOfService { get; init; }
    public string IdOfService { get; init; }
    public string Host { get; init; }
    public int Port { get; init; }
}