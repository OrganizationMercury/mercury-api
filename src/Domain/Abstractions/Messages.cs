namespace Domain.Abstractions;

public static class Messages
{
    public static string NotFound(string type, object id) => $"{type}: {id} was not found";
}