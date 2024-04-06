using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Aggregates.UserAggregate;

public class User : Entity
{
    public UserName UserName { get; private set; }
    public Birthday Birthday { get; private set; }

    private List<Guid> _publications;
    public IReadOnlyCollection<Guid> Publications => _publications.AsReadOnly();

    private User() { }

    public User(UserName userName, Birthday birthday) : this()
    {
        Id = Guid.NewGuid();
        UserName = userName;
        Birthday = birthday;
        _publications = new();
    }

    public static User AddUser(string firstName, string lastName, string birthday)
    {
        var userName = new UserName(firstName, lastName);
        var userBirthday = new Birthday(birthday);
        
        var newUser = new User(userName,userBirthday);
                        
        return newUser;
    }
}