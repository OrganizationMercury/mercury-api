using Domain.Abstractions;

namespace Domain.Exceptions;

public class AlreadyExistsException(string type, object id) : Exception(Messages.NotFound(type, id));
public class AlreadyExistsException<TType>(object id) : AlreadyExistsException(nameof(TType), id);