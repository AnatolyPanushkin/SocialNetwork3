using System.Globalization;
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

    public async Task<User> AddUser(UserInputDto userInputDto)
    {
        var user = User.AddUser(userInputDto.FirstName, userInputDto.LastName, userInputDto.Birthday);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<Publication> AddPublication(PublicationInputDto publicationInputDto)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == publicationInputDto.UserGuidId);

        var newPublication = Publication.AddNewPublication(user, publicationInputDto.TextContent, publicationInputDto.MediaContent);

        await _context.Publications.AddAsync(newPublication);
        await _context.SaveChangesAsync();

        return newPublication;
    }
}