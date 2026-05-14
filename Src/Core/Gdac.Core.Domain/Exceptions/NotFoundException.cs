namespace Gdac.Core.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string resource, object key)
        : base($"{resource} '{key}' não encontrado.") { }
}
