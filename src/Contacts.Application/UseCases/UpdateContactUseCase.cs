using Contacts.Application.DTOs;
using Contacts.Application.Interfaces;
using Contacts.Domain.Abstractions;
using Contacts.Domain.Exceptions;
using FluentValidation;

namespace Contacts.Application.UseCases;

/// <summary>
/// Orquestra a atualização de um contato ativo.
/// </summary>
public class UpdateContactUseCase
{
    private readonly IContactRepository _repository;
    private readonly IValidator<ContactRequest> _validator;
    private readonly IDateTimeProvider _clock;

    /// <summary>
    /// Inicializa o caso de uso com repositório, validador e relógio da aplicação.
    /// </summary>
    public UpdateContactUseCase(
        IContactRepository repository,
        IValidator<ContactRequest> validator,
        IDateTimeProvider clock)
    {
        _repository = repository;
        _validator = validator;
        _clock = clock;
    }

    /// <summary>
    /// Valida o request e atualiza o contato ativo identificado.
    /// </summary>
    public async Task<ContactResponse?> ExecuteAsync(Guid id, ContactRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new DomainException(validationResult.Errors.Select(e => e.ErrorMessage));

        var contact = await _repository.GetActiveByIdAsync(id, cancellationToken);
        if (contact is null)
            return null;

        var today = _clock.Today;
        var utcNow = _clock.UtcNow;

        contact.Update(request.Name, request.BirthDate, request.Sex!.Value, today, utcNow);
        await _repository.UpdateAsync(contact, cancellationToken);
        return ContactResponse.FromEntity(contact, today);
    }
}
