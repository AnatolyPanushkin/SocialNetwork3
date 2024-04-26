using MediatR;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Services.PublicationService;
using SocialNetwork.Domain.Aggregates.PublicationAggregate;

namespace SocialNetwork.Application.Features.GettingPublication;

public class GetPublication
{
    
}


public record GetPublicationQuery(string UserId, string OwnerOfPublicationId) : IRequest<List<PublicationDto>>;


public class GetPublicationQueryHadler : IRequestHandler<GetPublicationQuery, List<PublicationDto>>
{
    private readonly IPublicationService _publicationService;

    public GetPublicationQueryHadler(IPublicationService publicationService)
    {
        _publicationService = publicationService;
    }

    public async Task<List<PublicationDto>> Handle(GetPublicationQuery request, CancellationToken cancellationToken)
    {
        return await _publicationService.GetPublication(request.UserId, request.OwnerOfPublicationId);
    }
}