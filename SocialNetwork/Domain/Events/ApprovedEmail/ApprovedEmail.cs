using MediatR;
using SocialNetwork.Application.Services.UserServices;

namespace SocialNetwork.Domain.Events.ApprovedEmail;

public class ApprovedEmail
{
    
}

public record ApprovedEmailCommand(string Id) : IRequest;

public class ApprovedEmailCommandHandler(IUserService userService) : IRequestHandler<ApprovedEmailCommand>
{
    public async Task Handle(ApprovedEmailCommand request, CancellationToken cancellationToken)
    {
        await userService.ApproveEmail(request.Id);
    }
}