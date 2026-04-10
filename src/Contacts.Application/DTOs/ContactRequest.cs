using Contacts.Domain.Enums;

namespace Contacts.Application.DTOs;

/// <summary>
/// Representa o payload de entrada usado para criar ou atualizar um contato.
/// </summary>
public class ContactRequest
{
    public string Name { get; set; } = default!;

    public DateOnly BirthDate { get; set; }

    public Sex? Sex { get; set; }
}
