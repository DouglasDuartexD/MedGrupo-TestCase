using Contacts.Domain.Entities;

namespace Contacts.Application.Interfaces;

/// <summary>
/// Define as operações de persistência e consulta de contatos.
/// </summary>
public interface IContactRepository
{
    Task AddAsync(Contact contact, CancellationToken cancellationToken = default);

    Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Contact?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Contact>> ListActiveAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default);

    Task DeleteAsync(Contact contact, CancellationToken cancellationToken = default);
}
