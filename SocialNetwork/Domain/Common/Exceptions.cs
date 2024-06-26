﻿namespace SocialNetwork.Domain.Common;

public class Exceptions
{

}

public class UserNotFound : BadRequestException
{
    public UserNotFound() : base("User was not found")
    {
    }
}


public class IncorrectDateFormat : BadRequestException
{ 
    public IncorrectDateFormat() : base("Incorrect format of date")
    {
    }
}

public class EmptyContentField : BadRequestException
{
    public EmptyContentField() : base("Field of content is empty")
    {
    }
}

public class ContentLenghtIsTooLong : BadRequestException
{
    public ContentLenghtIsTooLong() : base("Lenght of content is too long")
    {
    }
}

public class AvailableRandomFriendsNotFound : BadRequestException
{
    public AvailableRandomFriendsNotFound() : base("There are no friends available for assignment")
    {
    }
}

public class InvalidEmailAddress : BadRequestException
{
    public InvalidEmailAddress() : base("Invalid email address")
    {
    }
}