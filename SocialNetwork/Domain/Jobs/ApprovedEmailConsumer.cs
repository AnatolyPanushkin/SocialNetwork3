using Confluent.Kafka;
using MediatR;
using Quartz;
using Quartz.Impl;
using SocialNetwork.Domain.Events.ApprovedEmail;

namespace SocialNetwork.Domain.Jobs;

public class ApprovedEmailConsumer : IJob
{
    private readonly string _topic = "ApprovedEmail-events";
    private readonly IMediator _mediator;
    public static readonly JobKey Key = new JobKey("ApprovedEmailConsumer");

    public ApprovedEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var consumerApprovedEmailConfig = new ConsumerConfig
        {
            BootstrapServers = $"localhost:29092",
            GroupId = "ApprovedEmail-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var builder = new ConsumerBuilder<Ignore,string>(consumerApprovedEmailConfig).Build())
        {
            builder.Subscribe(_topic);
            var cancelToken = new CancellationTokenSource();
            try
            {
                var consumer = builder.Consume(cancelToken.Token);
                _mediator.Send(new ApprovedEmailCommand(consumer.Message.Value));
            }
            catch (Exception)
            {
                builder.Close();
            }
        }

        return Task.CompletedTask;
    }
}
public class ApprovedEmailTransactionScheduler
{
    public static async Task Start()
    {
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        IJobDetail ApprovedEmailJob = JobBuilder.Create<ApprovedEmailConsumer>().Build();

        ITrigger ApprovedEmailTrigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(1)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(ApprovedEmailJob, ApprovedEmailTrigger);
    }
}

/*internal interface IScopedApprovedEmailService
{
    void ApproveEmail(CancellationToken stoppingToken);
}

internal class ScopedApprovedEmailService : IScopedApprovedEmailService
{
    private readonly string _topic = "ApprovedEmail-events";
    private readonly IMediator _mediator;

    public ScopedApprovedEmailService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void ApproveEmail(CancellationToken stoppingToken)
    {
        var consumerApprovedEmailConfig = new ConsumerConfig
        {
            BootstrapServers = $"localhost:29092",
            GroupId = "ApprovedEmail-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var builder = new ConsumerBuilder<Ignore,string>(consumerApprovedEmailConfig).Build())
        {
            builder.Subscribe(_topic);
            var cancelToken = new CancellationTokenSource();
            try
            {
                var consumer = builder.Consume(cancelToken.Token);
                _mediator.Send(new ApprovedEmailCommand(consumer.Message.Value));
            }
            catch (Exception)
            {
                builder.Close();
            }
        }
    }
}

public class ApproveEmailScopedServiceHostedService : BackgroundService
{
    public ApproveEmailScopedServiceHostedService(IServiceProvider services)
    {
        Services = services;
    }

    public IServiceProvider Services { get; }

      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        using (var scope = Services.CreateScope())
        {
            var scopedProcessingService = 
                scope.ServiceProvider
                    .GetRequiredService<IScopedApprovedEmailService>();

            scopedProcessingService.ApproveEmail(stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
    }
}

public class ApprovedEmailConsumer : IHostedService, IDisposable
{
    private readonly string _topic = "ApprovedEmail-events";
    private readonly IMediator _mediator;
    private Timer? _timer = null;
    
    public ApprovedEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ApproveEmail, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(10));

        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
    
    public void ApproveEmail(object? state)
    {
        var consumerApprovedEmailConfig = new ConsumerConfig
        {
            BootstrapServers = $"localhost:29092",
            GroupId = "ApprovedEmail-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var builder = new ConsumerBuilder<Ignore,string>(consumerApprovedEmailConfig).Build())
        {
            builder.Subscribe(_topic);
            var cancelToken = new CancellationTokenSource();
            try
            {
                var consumer = builder.Consume(cancelToken.Token);
                _mediator.Send(new ApprovedEmailCommand(consumer.Message.Value));
            }
            catch (Exception)
            {
                builder.Close();
            }
        }
    }
}*/