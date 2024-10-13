using Confluent.Kafka;
using MediatR;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Services.UserServices;

namespace SocialNetwork.Domain.Events.AddNewUser;

public class AddUserWithoutApprove
{
    
}

public record AddUserWithoutApproveCommand(UserInputWithEmailDto UserDto):IRequest<string>;

public class AddUserWithoutApproveCommandHandler(IUserService userService) : IRequestHandler<AddUserWithoutApproveCommand, string>
{
    public async Task<string> Handle(AddUserWithoutApproveCommand request, CancellationToken cancellationToken)
    {
        var user = await userService.AddUser(request.UserDto);
        
        return user.Id.ToString();
    }
}