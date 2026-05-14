using Gdac.Core.Domain.Enums;

namespace Gdac.Core.Domain.Entities;

public class CompanyMember
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = default!;

    public DateOnly? Since { get; private set; }

    public PersonType PersonType { get; private set; }
    // Internal UUID from CNPJ API (pessoa.id)
    public string? PersonExternalId { get; private set; }
    public string PersonName { get; private set; } = default!;
    // Masked CPF ("***.***.***-**") or full CNPJ for legal persons
    public string? PersonTaxId { get; private set; }
    // Age range from API, e.g. "41-50"
    public string? PersonAge { get; private set; }

    // Qualificação (role in the company)
    public int RoleId { get; private set; }
    public string RoleText { get; private set; } = default!;

    // Legal representative (only when PersonType = Legal)
    public string? AgentName { get; private set; }
    public string? AgentTaxId { get; private set; }
    public int? AgentRoleId { get; private set; }
    public string? AgentRoleText { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private CompanyMember() { }

    public static CompanyMember Create(
        Guid companyId,
        PersonType personType,
        string personName,
        int roleId,
        string roleText,
        DateOnly? since = null,
        string? personExternalId = null,
        string? personTaxId = null,
        string? personAge = null,
        string? agentName = null,
        string? agentTaxId = null,
        int? agentRoleId = null,
        string? agentRoleText = null)
    {
        return new CompanyMember
        {
            Id               = Guid.NewGuid(),
            CompanyId        = companyId,
            Since            = since,
            PersonType       = personType,
            PersonExternalId = personExternalId,
            PersonName       = personName.Trim(),
            PersonTaxId      = personTaxId,
            PersonAge        = personAge,
            RoleId           = roleId,
            RoleText         = roleText.Trim(),
            AgentName        = agentName?.Trim(),
            AgentTaxId       = agentTaxId,
            AgentRoleId      = agentRoleId,
            AgentRoleText    = agentRoleText?.Trim(),
            CreatedAt        = DateTime.UtcNow
        };
    }
}
