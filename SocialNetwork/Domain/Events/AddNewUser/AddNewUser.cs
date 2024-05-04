using Confluent.Kafka;
using MediatR;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Services.UserServices;
using SocialNetwork.Domain.Aggregates.UserAggregate;

namespace SocialNetwork.Domain.Events.AddNewUser;

public class AddNewUser
{
    
}

public record AddUserCommand(UserInputWithEmailDto UserDto):IRequest<string>;

public class AddUserCommandHandler(IProducer<string, string> producer, IUserService userService) : IRequestHandler<AddUserCommand, string>
{
    private readonly IUserService _userService = userService;
    private readonly IProducer<string, string> _producer = producer;
    private const string Topic = "addNewUser-events";
    
    public async Task<string> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.AddUser(request.UserDto);

        var kafkaMessage = new Message<string, string>
        {
            Key = user.Id.ToString(),
            Value = user.Email.Email.ToString()
        };
        
        await _producer.ProduceAsync(Topic, kafkaMessage);

        return "";
    }
}