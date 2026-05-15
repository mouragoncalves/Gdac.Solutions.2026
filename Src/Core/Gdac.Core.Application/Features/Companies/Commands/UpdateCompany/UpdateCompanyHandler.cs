using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.UpdateCompany;

public class UpdateCompanyHandler(ICompanyRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateCompanyCommand>
{
    public async Task Handle(UpdateCompanyCommand request, CancellationToken ct)
    {
        var company = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Empresa", request.Id);

        company.Update(request.Name, request.TradeName, request.Cnpj,
            request.Type, request.Email, request.Phone,
            request.Segment, request.SizeCategory);
        repo.Update(company);
        await uow.CommitAsync(ct);
    }
}
