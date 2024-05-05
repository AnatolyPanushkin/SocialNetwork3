using SocialNetwork.Domain.Aggregates.PublicationAggregate;
using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Aggregates.UserAggregate;

public class User : Entity
{
    public UserName UserName { get; private set; }
    public Birthday Birthday { get; private set; }
    public EmailAddress Email { get; set; }

    private List<Publication> _publications;
    public IReadOnlyCollection<Publication> Publications => _publications.AsReadOnly();
    
    public bool ApprovedEmail { get; private set; }
    
    public bool IsBanned { get; private set; }

    private User() { }

    public User(UserName userName, Birthday birthday) : this()
    {
        Id = Guid.NewGuid();
        UserName = userName;
        Birthday = birthday;
        _publications = new();
    }
    
    public User(UserName userName, Birthday birthday, EmailAddress email) : this()
    {
        Id = Guid.NewGuid();
        UserName = userName;
        Birthday = birthday;
        Email = email;
        _publications = new();
    }

    public User(string firstName, string lastName, string birthday, string email)
    {
        Id = Guid.NewGuid();
        UserName = new UserName(firstName, lastName);
        Birthday = new Birthday(birthday);
        Email = new EmailAddress(email);
        _publications = new();
        ApprovedEmail = false;
        IsBanned = false;
    }

    public static User AddUser(string firstName, string lastName, string birthday)
    {
        var userName = new UserName(firstName, lastName);
        var userBirthday = new Birthday(birthday);
        
        var newUser = new User(userName,userBirthday);
                        
        return newUser;
    }

    public static User ApproveEmail(User user)
    {
        user.ApprovedEmail = true;

        return user;
    }

    public static User ReportUser(User? reportedUser)
    {
        if (reportedUser is null)
        {
            throw new UserNotFound();
        }
        
        reportedUser.IsBanned = true;

        return reportedUser;
    }
}