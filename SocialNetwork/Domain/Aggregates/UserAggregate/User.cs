using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Aggregates.UserAggregate;

public class User : Entity
{
    public UserName UserName { get; private set; }
    public Birthday Birthday { get; private set; }

    private List<Publication> _publications;
    public IReadOnlyCollection<Publication> Publications => _publications.AsReadOnly();

    private virtual ICollection<UsersFriends> _usersFriends;
    public IReadOnlyCollection<UsersFriends> UsersFriends => _usersFriends.AsReadOnly();

    /* private List<User> _friends;
     public IReadOnlyCollection<User> Friends => _friends.AsReadOnly();*/

    private User() { }

    public User(UserName userName, Birthday birthday) : this()
    {
        Id = Guid.NewGuid();
        UserName = userName;
        Birthday = birthday;
        _publications = new List<Publication>();
        //_friends = new List<User>();
    }
}