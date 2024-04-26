using System.Runtime.InteropServices.JavaScript;
using SocialNetwork.Domain.Aggregates.UserAggregate;
using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Aggregates.PublicationAggregate;

public class Publication : Entity
{
    public TextContent TextContent { get; private set; }
    public string MediaContent { get; private set; }
    
    public User User { get; }

    private Publication() {}
    
    public Publication(TextContent textContent, string mediaContent)
    {
        Id = Guid.NewGuid();
        TextContent = textContent?? throw new ArgumentNullException(nameof(textContent));
        MediaContent = mediaContent?? throw new ArgumentNullException(nameof(mediaContent));
    }
    public Publication(TextContent textContent, string mediaContent, User user)
    {
        Id = Guid.NewGuid();
        TextContent = textContent?? throw new ArgumentNullException(nameof(textContent));
        MediaContent = mediaContent?? throw new ArgumentNullException(nameof(mediaContent));
        this.User = user ?? throw new ArgumentNullException(nameof(user));
    }

    public static Publication AddNewPublication(User user, string textContent, string mediaContent)
    {
        if (user is null) throw new UserNotFound();

        var newTextContent = new TextContent(textContent);
        
        var newPublication = new Publication(newTextContent, mediaContent, user);
        
        return newPublication;
    }

    public static List<Publication> GetPublicationsOfUser(List<RandomFriend> randomFriendsOfUserList, string ownerOfPublicationId, List<Publication> publications)
    {
        var currentFriend =
            randomFriendsOfUserList.FirstOrDefault(user =>
                user.ExpirationTime == ToDateOnly(DateTime.Now.AddDays(180)));

        if (currentFriend.RandomFriendOfUser != Guid.Parse(ownerOfPublicationId))
        {
            return new List<Publication>();
        }

        return publications;
    }
    
    private static DateOnly ToDateOnly(DateTime dateTime)
    {
        return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
    }
}