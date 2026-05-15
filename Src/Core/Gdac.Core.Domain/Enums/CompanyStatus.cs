namespace Gdac.Core.Domain.Enums;

public enum CompanyStatus
{
    Prospect  = 1,
    Active    = 2,
    Overdue   = 3,   // Inadimplente (>5 dias corridos sem pagamento)
    Blocked   = 4,   // Bloqueado manualmente pela GDAC
    Inactive  = 5    // Inativo / soft-deleted
}
