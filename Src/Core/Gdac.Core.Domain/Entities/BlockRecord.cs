namespace Gdac.Core.Domain.Entities;

public class BlockRecord
{
    public Guid Id { get; private set; }

    // CNPJ raiz (8 dígitos) — bloqueia o grupo econômico inteiro
    public string CnpjBase { get; private set; } = default!;

    public string? Reason { get; private set; }
    public Guid BlockedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private BlockRecord() { }

    public static BlockRecord Create(string cnpjBase, Guid blockedBy, string? reason = null)
    {
        return new BlockRecord
        {
            Id        = Guid.NewGuid(),
            CnpjBase  = cnpjBase.Trim(),
            Reason    = reason?.Trim(),
            BlockedBy = blockedBy,
            CreatedAt = DateTime.UtcNow
        };
    }
}
