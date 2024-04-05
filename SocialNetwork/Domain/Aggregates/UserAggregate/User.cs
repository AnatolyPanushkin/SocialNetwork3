using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Aggregates.UserAggregate;

public class User : Entity
{
    public UserName UserName { get; private set; }
    public Birthday Birthday { get; private set; }

    private List<Guid> _publications;
    public IReadOnlyCollection<Guid> Publications => _publications.AsReadOnly();

    private List<Guid> _friends;
    public IReadOnlyCollection<Guid> Friends => _friends.AsReadOnly();

    private User() { }

    public User(UserName userName, Birthday birthday) : this()
    {
        Id = Guid.NewGuid();
        UserName = userName;
        Birthday = birthday;
    }
}