using SocialNetwork.Application.DTOs;
using SocialNetwork.Domain.Aggregates.PublicationAggregate;

namespace SocialNetwork.Application.Services.PublicationService;

public interface IPublicationService
{
    public Task<List<PublicationDto>> GetPublication(string userId, string ownerOfPublicationId);
}