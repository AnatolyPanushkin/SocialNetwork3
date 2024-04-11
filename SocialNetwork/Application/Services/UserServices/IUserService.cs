using SocialNetwork.Application.DTOs;
using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Domain.Aggregates.PublicationAggregate;
using SocialNetwork.Domain.Aggregates.UserAggregate;

namespace SocialNetwork.Application.Services.UserServices;

public interface IUserService
{
    public Task<User> AddUser(UserInputDto userInputDto);
    public Task<Publication> AddPublication(string textContent, string mediaContent, Guid userId);
}