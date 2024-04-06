namespace Domain.Abstractions;

public record Error(string Message)
{
    public static NotFoundError NotFound(string type, string id) => new(type, id);
}

public sealed record NotFoundError : Error
{
    public NotFoundError(string type, string id) : base(Messages.NotFound(type, id)){}
}