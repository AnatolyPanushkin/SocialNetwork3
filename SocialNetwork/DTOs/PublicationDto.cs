namespace SocialNetwork.DTOs;

public class PublicationDto
{
}

public class PublicationInputDto
{
    public string TextContent { get; set; }
    public string MediaContent { get; set; }
    public Guid UserGuidId { get; set; }
}