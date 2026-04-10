using Contacts.Application.Interfaces;
using Contacts.Domain.Entities;
using Contacts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Repositories;

/// <summary>
/// Implementação de repositório de contatos baseada em Entity Framework Core.
/// </summary>
public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto de persistência.
    /// </summary>
    public ContactRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona um novo contato e persiste a transação.
    /// </summary>
    public async Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        await _context.Contacts.AddAsync(contact, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Busca um contato pelo identificador, independente do status.
    /// </summary>
    public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts.FindAsync([id], cancellationToken);
    }

    /// <summary>
    /// Busca um contato ativo pelo identificador.
    /// </summary>
    public async Task<Contact?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts
            .Where(c => c.Id == id && c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retorna a lista de contatos ativos ordenada por nome.
    /// </summary>
    public async Task<IReadOnlyList<Contact>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Contacts
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Persiste alterações de um contato existente.
    /// </summary>
    public async Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Remove fisicamente um contato do banco.
    /// </summary>
    public async Task DeleteAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
