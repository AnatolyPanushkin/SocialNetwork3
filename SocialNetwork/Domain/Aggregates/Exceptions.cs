using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Aggregates;

public class Exceptions
{
    
}

public class UserNotFound : BadRequestException
{
    public UserNotFound() : base("User was not found ")
    {
    }
}