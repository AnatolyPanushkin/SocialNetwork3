using MediatR;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Services.PublicationService;

namespace SocialNetwork.Application.Features.GettingPublication;

public class GetPublicationByUserId
{
    
}

public record GetPublicationByUserIdQuery(string OwnerOfPublicationId) : IRequest<List<PublicationWithUserDto>>;

public class GetPublicationByUserIdQueryHandler : IRequestHandler<GetPublicationByUserIdQuery, List<PublicationWithUserDto>>
{
    private readonly IPublicationService _publicationService;

    public GetPublicationByUserIdQueryHandler(IPublicationService publicationService)
    {
        _publicationService = publicationService;
    }

    public async Task<List<PublicationWithUserDto>> Handle(GetPublicationByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _publicationService.GetPublicationByUserId(request.OwnerOfPublicationId);
    }
}