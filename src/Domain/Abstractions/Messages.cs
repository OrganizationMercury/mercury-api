namespace Domain.Abstractions;

public static class Messages
{
    public static string NotFound(string type, object id) => $"{type}: {id} was not found";
    public static string NotFound<T>(object id) => $"{nameof(T)}: {id} was not found";
}