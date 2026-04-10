using Contacts.Application.Interfaces;

namespace Contacts.Application.UseCases;

/// <summary>
/// Remove definitivamente um contato.
/// </summary>
public class DeleteContactUseCase
{
    private readonly IContactRepository _repository;

    /// <summary>
    /// Inicializa o caso de uso com o repositório de contatos.
    /// </summary>
    public DeleteContactUseCase(IContactRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Exclui o contato se ele existir.
    /// </summary>
    public async Task<bool> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contact = await _repository.GetByIdAsync(id, cancellationToken);
        if (contact is null)
            return false;

        await _repository.DeleteAsync(contact, cancellationToken);
        return true;
    }
}
