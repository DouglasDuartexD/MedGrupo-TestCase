using Contacts.Application.DTOs;
using Contacts.Application.Interfaces;
using Contacts.Domain.Abstractions;
using Contacts.Domain.Entities;
using Contacts.Domain.Exceptions;
using FluentValidation;

namespace Contacts.Application.UseCases;

/// <summary>
/// Orquestra a criação de um novo contato.
/// </summary>
public class CreateContactUseCase
{
    private readonly IContactRepository _repository;
    private readonly IValidator<ContactRequest> _validator;
    private readonly IDateTimeProvider _clock;

    /// <summary>
    /// Inicializa o caso de uso com repositório, validador e relógio da aplicação.
    /// </summary>
    public CreateContactUseCase(
        IContactRepository repository,
        IValidator<ContactRequest> validator,
        IDateTimeProvider clock)
    {
        _repository = repository;
        _validator = validator;
        _clock = clock;
    }

    /// <summary>
    /// Valida, cria e persiste um novo contato.
    /// </summary>
    public async Task<ContactResponse> ExecuteAsync(ContactRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new DomainException(validationResult.Errors.Select(e => e.ErrorMessage));

        // Captura a referência temporal uma única vez para toda a operação.
        var today = _clock.Today;
        var utcNow = _clock.UtcNow;

        var contact = Contact.Create(request.Name, request.BirthDate, request.Sex!.Value, today, utcNow);
        await _repository.AddAsync(contact, cancellationToken);
        return ContactResponse.FromEntity(contact, today);
    }
}
