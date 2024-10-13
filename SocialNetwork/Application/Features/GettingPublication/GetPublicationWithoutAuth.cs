using MediatR;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Application.Services.PublicationService;

namespace SocialNetwork.Application.Features.GettingPublication
{
    public class GetPublicationWithoutAuth
    {
    }

    public record GetPublicationWithoutAuthQuery() : IRequest<List<PublicationWithUserDto>>;

    public class GetPublicationWithoutAuthQueryHandler : IRequestHandler<GetPublicationWithoutAuthQuery, List<PublicationWithUserDto>>
    {
        private readonly IPublicationService _publicationService;

        public GetPublicationWithoutAuthQueryHandler(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        public Task<List<PublicationWithUserDto>> Handle(GetPublicationWithoutAuthQuery request, CancellationToken cancellationToken)
        {
           return _publicationService.GetPublicationWithoutAuth();
        }
    }
}
