using Confluent.Kafka;
using MediatR;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Domain.Aggregates.UserAggregate;

namespace SocialNetwork.Domain.Events.AddNewUser;

public class AddNewUser
{
    
}

public record AddUserCommand(UserDto UserDto):IRequest<string>;

public class AddUserCommandHandler(IProducer<string, string> producer) : IRequestHandler<AddUserCommand, string>
{
    private readonly IProducer<string, string> _producer = producer;
    private const string Topic = "approveEmail-events";
    
    public async Task<string> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var userEmailAddress = new EmailAddress(request.UserDto.Email);
        
        var kafkaMessage = new Message<string, string>
        {
            Value = userEmailAddress.Email
        };
        
        await _producer.ProduceAsync(Topic, kafkaMessage);

        return "";
    }
}