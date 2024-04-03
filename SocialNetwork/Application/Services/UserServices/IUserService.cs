using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Domain.Aggregates.UserAggregate;

namespace SocialNetwork.Application.Services.UserServices;

public interface IUserService
{
    public Task<User> AddUser(string firstName, string lastName, DateOnly birthday);
    public Task<Publication> AddPublication(string textContent, string mediaContent, Guid userId);

    public Task<List<Publication>> GetListPublications(Guid currentUserId, Guid userId);
}