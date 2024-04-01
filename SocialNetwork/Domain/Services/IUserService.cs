using SocialNetwork.Domain.Aggregates;

namespace SocialNetwork.Domain.Services;

public interface IUserService
{
    public Task<User> AddUser(string firstName, string lastName, DateOnly birthday);
    public Task<Publication> AddPublication(string textContent, string mediaContent, Guid userId);

    public Task<List<Publication>> GetListPublications(Guid currentUserId, Guid userId);
}