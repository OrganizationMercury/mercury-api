namespace Api.Dto;

public record UpdateUserDto(
    Guid Id,
    string Firstname,
    string Lastname,
    string Username,
    string? Bio,
    IFormFile? File);
