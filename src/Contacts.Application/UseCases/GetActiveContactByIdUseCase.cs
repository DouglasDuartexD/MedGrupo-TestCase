using Contacts.Application.DTOs;
using Contacts.Application.Interfaces;
using Contacts.Domain.Abstractions;

namespace Contacts.Application.UseCases;

/// <summary>
/// Busca um contato ativo pelo identificador.
/// </summary>
public class GetActiveContactByIdUseCase
{
    private readonly IContactRepository _repository;
    private readonly IDateTimeProvider _clock;

    /// <summary>
    /// Inicializa o caso de uso com repositório e data de referência.
    /// </summary>
    public GetActiveContactByIdUseCase(IContactRepository repository, IDateTimeProvider clock)
    {
        _repository = repository;
        _clock = clock;
    }

    /// <summary>
    /// Retorna o contato ativo correspondente ao identificador informado.
    /// </summary>
    public async Task<ContactResponse?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contact = await _repository.GetActiveByIdAsync(id, cancellationToken);
        return contact is null ? null : ContactResponse.FromEntity(contact, _clock.Today);
    }
}
