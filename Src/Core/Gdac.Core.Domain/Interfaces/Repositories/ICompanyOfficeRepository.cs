using Gdac.Core.Domain.Entities;

namespace Gdac.Core.Domain.Interfaces.Repositories;

public interface ICompanyOfficeRepository
{
    Task<IReadOnlyList<CompanyOffice>> GetByCompanyIdAsync(Guid companyId, CancellationToken ct = default);
    Task<CompanyOffice?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(CompanyOffice office, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<CompanyOffice> offices, CancellationToken ct = default);
    void Remove(CompanyOffice office);
    Task RemoveAllByCompanyIdAsync(Guid companyId, CancellationToken ct = default);
}
