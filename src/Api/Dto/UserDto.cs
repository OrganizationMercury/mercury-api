namespace Api.Dto;

public record CreateUserDto(string Firstname, string Lastname, string Username);

public record UpdateUserDto(
    Guid Id,
    string Firstname,
    string Lastname,
    string Username,
    string? Bio,
    IFormFile? File);
