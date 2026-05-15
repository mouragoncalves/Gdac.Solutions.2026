using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyHandler(ICompanyRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateCompanyCommand, Guid>
{
    public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken ct)
    {
        if (request.Cnpj is not null && await repo.ExistsByCnpjAsync(request.Cnpj, ct))
            throw new DomainException("Já existe uma empresa com este CNPJ.");

        var company = Company.Create(request.Name, request.Type,
            request.TradeName, request.Cnpj, request.Email, request.Phone,
            request.Segment, request.SizeCategory);

        await repo.AddAsync(company, ct);
        await uow.CommitAsync(ct);
        return company.Id;
    }
}
