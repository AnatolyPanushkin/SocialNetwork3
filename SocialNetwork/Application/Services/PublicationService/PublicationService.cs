﻿using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.DTOs;
using SocialNetwork.Domain.Aggregates.PublicationAggregate;
using SocialNetwork.Infrastructure.Data;

namespace SocialNetwork.Application.Services.PublicationService;

public class PublicationService : IPublicationService
{
    private readonly SocialNetworkContext _context;

    public PublicationService(SocialNetworkContext context)
    {
        _context = context;
        
    }
    
    public async Task<List<PublicationDto>> GetPublication(string userId, string ownerOfPublicationId)
    {
        var randomFriendsOfUserList = await _context.RandomFriends.Where(randFriend => randFriend.User == Guid.Parse(userId)).ToListAsync();

        var publications = await _context.Publications
            .Where(publication => publication.User.Id == Guid.Parse(ownerOfPublicationId)).ToListAsync();
        
        return Publication.GetPublicationsOfUser(randomFriendsOfUserList, ownerOfPublicationId, publications)
            .Select(publication=> new PublicationDto(publication.Id.ToString(),publication.TextContent,publication.MediaContent))
            .ToList();
    }

    public async Task<List<PublicationWithUserDto>> GetPublicationByUserId(string userId)
    {
        return await _context.Publications.AsNoTracking().Where(p => p.UserId == Guid.Parse(userId))
            .Select(p => new PublicationWithUserDto(p.Id.ToString(), p.UserId.ToString(), p.TextContent.Content, p.MediaContent)).ToListAsync();       
    }

    public async Task<List<PublicationWithUserDto>> GetPublicationWithoutAuth()
    {
        var result =  await _context.Publications.AsNoTracking().Select(publication => new PublicationWithUserDto(publication.Id.ToString(), publication.UserId.ToString(), publication.TextContent.Content, publication.MediaContent)).ToListAsync();
        return result;
    }
}