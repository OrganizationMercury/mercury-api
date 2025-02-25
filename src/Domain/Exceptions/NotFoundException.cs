using Domain.Abstractions;

namespace Domain.Exceptions;

public class NotFoundException(string type, object id) : Exception(Messages.NotFound(type, id));