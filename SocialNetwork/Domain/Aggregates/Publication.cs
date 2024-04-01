using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Aggregates;

public class Publication : Entity
{
    public string TextContent { get; private set; }
    public string MediaContent { get; private set; }

    public User User { get; }

    private Publication() {}
    
    public Publication(string textContent, string mediaContent)
    {
        Id = new Guid();
        TextContent = textContent?? throw new ArgumentNullException(nameof(textContent));
        MediaContent = mediaContent?? throw new ArgumentNullException(nameof(mediaContent));
    }
    public Publication(string textContent, string mediaContent, User user)
    {
        Id = new Guid();
        TextContent = textContent?? throw new ArgumentNullException(nameof(textContent));
        MediaContent = mediaContent?? throw new ArgumentNullException(nameof(mediaContent));
        this.User = user ?? throw new ArgumentNullException(nameof(user));
    }
}