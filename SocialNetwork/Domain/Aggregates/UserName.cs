using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Aggregates;

public class UserName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    public UserName(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}