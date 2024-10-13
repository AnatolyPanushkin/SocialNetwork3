using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Domain.Aggregates.PublicationAggregate;
using SocialNetwork.Domain.Aggregates.UserAggregate;
using SocialNetwork.Domain.Common;
using SocialNetwork.Infrastructure.Data;

namespace SocialNetwork.Application.Services.UserServices;

public class UserService : IUserService
{
    private readonly SocialNetworkContext _context;

    public UserService(SocialNetworkContext context)
    {
        _context = context;
    }

    public async Task<User> AddUser(UserInputWithEmailDto userDto)
    {
        var user = new User(userDto.FirstName,userDto.LastName, userDto.Birthday, userDto.Email);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task ApproveEmail(string Id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(Id));

        var approvedUser = User.ApproveEmail(user);

        _context.Users.Update(approvedUser);
        await _context.SaveChangesAsync();
    }

    public async Task ReportUser(ReportUserDto reportUserDto)
    {
        var reportedUser = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(reportUserDto.ReportedUserId));
        
        var result = User.ReportUser(reportedUser);
        
        _context.Users.Update(result);
        await _context.SaveChangesAsync();
    }

    public async Task<Publication> AddPublication(PublicationInputDto publicationInputDto)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == Guid.Parse(publicationInputDto.UserGuidId));
        if (user is null) throw new UserNotFound();  

        var newPublication = Publication.AddNewPublication(publicationInputDto.UserGuidId, publicationInputDto.TextContent, publicationInputDto.MediaContent);

        await _context.Publications.AddAsync(newPublication);
        await _context.SaveChangesAsync();

        return newPublication;
    }
}