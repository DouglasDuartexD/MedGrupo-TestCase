using Contacts.Application.DTOs;
using Contacts.Domain.Abstractions;
using Contacts.Domain.Entities;
using Contacts.Domain.Enums;
using FluentValidation;

namespace Contacts.Application.Validators;

/// <summary>
/// Valida o payload de criação e atualização de contatos.
/// </summary>
public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
    /// <summary>
    /// Configura as regras de validação do request com base na data de referência atual.
    /// </summary>
    public ContactRequestValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

        // ApplyConditionTo.CurrentValidator garante que cada .When()
        // afeta apenas a regra imediatamente anterior na cadeia,
        // e não retroativamente todas as regras anteriores.
        RuleFor(x => x.BirthDate)
            .Must(d => d != DateOnly.MinValue)
                .WithMessage("BirthDate is required.")
            .Must(d => d <= dateTimeProvider.Today)
                .When(x => x.BirthDate != DateOnly.MinValue, ApplyConditionTo.CurrentValidator)
                .WithMessage("BirthDate cannot be greater than today.")
            .Must(d => Contact.CalculateAge(d, dateTimeProvider.Today) != 0)
                .When(x => x.BirthDate != DateOnly.MinValue && x.BirthDate <= dateTimeProvider.Today, ApplyConditionTo.CurrentValidator)
                .WithMessage("Age cannot be zero.")
            .Must(d => Contact.CalculateAge(d, dateTimeProvider.Today) >= 18)
                .When(x => x.BirthDate != DateOnly.MinValue && x.BirthDate <= dateTimeProvider.Today, ApplyConditionTo.CurrentValidator)
                .WithMessage("Contact must be 18 years old or older.");

        RuleFor(x => x.Sex)
            .NotNull().WithMessage("Sex is required.")
            .IsInEnum().WithMessage("Sex has an invalid value.");
    }
}
