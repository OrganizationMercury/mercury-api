namespace Api.Dto;

public record RegisterDto(string FirstName, string LastName, string UserName, string Password);

public record LoginDto(string UserName, string Password);
