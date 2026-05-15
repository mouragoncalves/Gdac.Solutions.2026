namespace Gdac.Onboarding.Domain.Exceptions;

public class NotFoundException(string entity, object id)
    : Exception($"{entity} '{id}' não encontrado.");
