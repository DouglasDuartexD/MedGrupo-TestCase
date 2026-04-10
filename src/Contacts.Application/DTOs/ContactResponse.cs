using Contacts.Domain.Entities;
using Contacts.Domain.Enums;

namespace Contacts.Application.DTOs;

/// <summary>
/// Representa os dados retornados pela API para um contato.
/// </summary>
public class ContactResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

    public DateOnly BirthDate { get; set; }

    public Sex Sex { get; set; }

    public int Age { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Converte uma entidade de domínio em um DTO de resposta.
    /// </summary>
    /// <param name="today">Data de referência para o cálculo de idade em tempo de execução.</param>
    public static ContactResponse FromEntity(Contact contact, DateOnly today) => new()
    {
        Id = contact.Id,
        Name = contact.Name,
        BirthDate = contact.BirthDate,
        Sex = contact.Sex,
        Age = Contact.CalculateAge(contact.BirthDate, today),
        IsActive = contact.IsActive,
        CreatedAt = contact.CreatedAt,
        UpdatedAt = contact.UpdatedAt
    };
}
