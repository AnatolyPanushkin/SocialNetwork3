namespace SocialNetwork.Domain.Common;

public class Exceptions
{

}

public class UserNotFound : BadRequestException
{
    public UserNotFound() : base("User was not found ")
    {
    }
}

