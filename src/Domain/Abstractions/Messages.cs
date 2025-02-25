namespace Domain.Abstractions;

public static class Messages
{
    public static string NotFound(string type, object id) => $"{type}: {id} was not found";
    public static string NotFound<T>(object id) => $"{nameof(T)}: {id} was not found";
    
    public static string AlreadyExists(string type, object id) => $"{type}: {id} already exists";
    public static string AlreadyExists<T>(object id) => $"{nameof(T)}: {id} already exists";

    public const string UserCreated = "User created successfully";
    public const string IncorrectPassword = "Incorrect password";
}