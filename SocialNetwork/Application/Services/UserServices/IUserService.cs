using SocialNetwork.Application.DTOs;
using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Domain.Aggregates.PublicationAggregate;
using SocialNetwork.Domain.Aggregates.UserAggregate;

namespace SocialNetwork.Application.Services.UserServices;

public interface IUserService
{
    public Task<User> AddUser(UserInputWithEmailDto userDto);
    public Task ApproveEmail(string Id);
    public Task ReportUser(ReportUserDto reportUserDto);
    public Task<Publication> AddPublication(PublicationInputDto publicationInputDto);
}