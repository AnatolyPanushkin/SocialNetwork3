using MediatR;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Services.UserServices;

namespace SocialNetwork.Domain.Events.ReportUser;

public class ReportUser
{
    
}

public record ReportUserCommand(ReportUserDto ReportUserDto) : IRequest;

public class ReportUserCommandHandler(IUserService userService):IRequestHandler<ReportUserCommand>
{
    private readonly IUserService _userService = userService;
    
    public async Task Handle(ReportUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.ReportUser(request.ReportUserDto);
        //Добавить поиск нового рандомного друга
    }
}