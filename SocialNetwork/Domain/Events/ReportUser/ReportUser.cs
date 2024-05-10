using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Services.UserServices;
using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Domain.Common;
using SocialNetwork.Infrastructure.Data;

namespace SocialNetwork.Domain.Events.ReportUser;

public class ReportUser
{
    
}

public record ReportUserCommand(ReportUserDto ReportUserDto) : IRequest;

public class ReportUserCommandHandler(IUserService userService, SocialNetworkContext context):IRequestHandler<ReportUserCommand>
{
    private readonly SocialNetworkContext _context = context;
    private readonly IUserService _userService = userService;

    public async Task Handle(ReportUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.ReportUser(request.ReportUserDto);
        
        var randomFriendsOfUser = await _context.RandomFriends.Where(randFriends => randFriends.User == Guid.Parse(request.ReportUserDto.UserId))
            .Select(randFriends => randFriends.RandomFriendOfUser).ToListAsync(cancellationToken: cancellationToken);

        var randomUsersList = _context.Users.Where(u => u.Id != Guid.Parse(request.ReportUserDto.UserId) && !randomFriendsOfUser.Contains(u.Id))
            .ToList();

        if (!randomUsersList.Any())
        {
            throw new AvailableRandomFriendsNotFound();
        }

        var randomUser = randomUsersList[new Random().Next(randomUsersList.Count())];
                
        var newRandomFriend = new RandomFriend(request.ReportUserDto.UserId, randomUser.Id.ToString());
        var newFriendForRandom = new RandomFriend(randomUser.Id.ToString(),request.ReportUserDto.UserId);
        await _context.RandomFriends.AddAsync(newRandomFriend);
        await _context.RandomFriends.AddAsync(newFriendForRandom);
        await _context.SaveChangesAsync();
    }
}
    