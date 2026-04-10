using Contacts.Application.DTOs;
using Contacts.Application.Interfaces;
using Contacts.Domain.Abstractions;
using Contacts.Domain.Exceptions;

namespace Contacts.Application.UseCases;

/// <summary>
/// Desativa logicamente um contato existente.
/// </summary>
public class DeactivateContactUseCase
{
    private readonly IContactRepository _repository;
    private readonly IDateTimeProvider _clock;

    /// <summary>
    /// Inicializa o caso de uso com repositório e relógio da aplicação.
    /// </summary>
    public DeactivateContactUseCase(IContactRepository repository, IDateTimeProvider clock)
    {
        _repository = repository;
        _clock = clock;
    }

    /// <summary>
    /// Desativa o contato informado se ele existir e estiver ativo.
    /// </summary>
    public async Task<ContactResponse?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contact = await _repository.GetByIdAsync(id, cancellationToken);
        if (contact is null)
            return null;

        if (!contact.IsActive)
            throw new ConflictException("Contact is already inactive.");

        var utcNow = _clock.UtcNow;
        contact.Deactivate(utcNow);
        await _repository.UpdateAsync(contact, cancellationToken);
        return ContactResponse.FromEntity(contact, _clock.Today);
    }
}
