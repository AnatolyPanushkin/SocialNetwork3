namespace SocialNetwork.Application.DTOs;

public record UserDto(string FirstName, string LastName, string Birthday, string Email);

public record ApproveEmailUserDto(string Email);

public class UserInputDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Birthday { get; set; }
}

public class UserInputWithEmailDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Birthday { get; set; }
    public string Email { get; set; }
}

public class ReportUserDto
{
    public string UserId { get; set; }
    public string ReportedUserId { get; set; }
}