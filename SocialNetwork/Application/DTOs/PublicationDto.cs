﻿using SocialNetwork.Domain.Aggregates.PublicationAggregate;

namespace SocialNetwork.Application.DTOs;

public record PublicationDto(string Id, TextContent TextContent, string MediaContent);

public class PublicationInputDto
{
    public string TextContent { get; set; }
    public string MediaContent { get; set; }
    public Guid UserGuidId { get; set; }
}