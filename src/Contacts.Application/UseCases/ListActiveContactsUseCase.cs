using Contacts.Application.DTOs;
using Contacts.Application.Interfaces;
using Contacts.Domain.Abstractions;

namespace Contacts.Application.UseCases;

/// <summary>
/// Lista os contatos atualmente ativos.
/// </summary>
public class ListActiveContactsUseCase
{
    private readonly IContactRepository _repository;
    private readonly IDateTimeProvider _clock;

    /// <summary>
    /// Inicializa o caso de uso com repositório e data de referência.
    /// </summary>
    public ListActiveContactsUseCase(IContactRepository repository, IDateTimeProvider clock)
    {
        _repository = repository;
        _clock = clock;
    }

    /// <summary>
    /// Retorna a lista de contatos ativos convertida para DTOs de resposta.
    /// </summary>
    public async Task<IReadOnlyList<ContactResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var today = _clock.Today;
        var contacts = await _repository.ListActiveAsync(cancellationToken);
        return contacts.Select(c => ContactResponse.FromEntity(c, today)).ToList().AsReadOnly();
    }
}
