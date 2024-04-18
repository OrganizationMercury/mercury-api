using Domain.Abstractions;

namespace Domain.Exceptions;

public class NotFoundException(string type, string id) : Exception(Messages.NotFound(type, id));