namespace SocialNetwork.Application.DTOs;

public record UserDto(string FirstName, string LastName, string Birthday, string Email);

public record ApproveEmailUserDto(string Email);

public class UserInputDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Birthday { get; set; }
}