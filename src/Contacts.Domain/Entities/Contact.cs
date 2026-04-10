using Contacts.Domain.Enums;
using Contacts.Domain.Exceptions;

namespace Contacts.Domain.Entities;

/// <summary>
/// Entidade de domínio que representa um contato e suas regras centrais de negócio.
/// </summary>
public class Contact
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public DateOnly BirthDate { get; private set; }
    public Sex Sex { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Contact() { }

    /// <summary>
    /// Cria um novo contato válido aplicando todas as regras de negócio.
    /// </summary>
    /// <param name="today">Data de referência usada nas validações de negócio.</param>
    /// <param name="utcNow">Timestamp da operação usado em CreatedAt e UpdatedAt.</param>
    public static Contact Create(string name, DateOnly birthDate, Sex sex, DateOnly today, DateTime utcNow)
    {
        var errors = Validate(name, birthDate, today);
        if (errors.Count > 0)
            throw new DomainException(errors);

        return new Contact
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            BirthDate = birthDate,
            Sex = sex,
            IsActive = true,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };
    }

    /// <summary>
    /// Atualiza os dados de um contato existente aplicando as regras de negócio.
    /// </summary>
    /// <param name="today">Data de referência usada nas validações de negócio.</param>
    /// <param name="utcNow">Timestamp da operação usado em UpdatedAt.</param>
    public void Update(string name, DateOnly birthDate, Sex sex, DateOnly today, DateTime utcNow)
    {
        var errors = Validate(name, birthDate, today);
        if (errors.Count > 0)
            throw new DomainException(errors);

        Name = name.Trim();
        BirthDate = birthDate;
        Sex = sex;
        UpdatedAt = utcNow;
    }

    /// <summary>
    /// Desativa logicamente o contato.
    /// </summary>
    public void Deactivate(DateTime utcNow)
    {
        IsActive = false;
        UpdatedAt = utcNow;
    }

    /// <summary>
    /// Reativa um contato previamente desativado.
    /// </summary>
    public void Activate(DateTime utcNow)
    {
        IsActive = true;
        UpdatedAt = utcNow;
    }

    private static List<string> Validate(string name, DateOnly birthDate, DateOnly today)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Name is required.");

        if (birthDate > today)
            errors.Add("BirthDate cannot be greater than today.");

        var age = CalculateAge(birthDate, today);

        if (age == 0)
            errors.Add("Age cannot be zero.");

        if (age < 18)
            errors.Add("Contact must be 18 years old or older.");

        return errors;
    }

    /// <summary>
    /// Calcula a idade em anos completos a partir de uma data de referência explícita.
    /// </summary>
    public static int CalculateAge(DateOnly birthDate, DateOnly today)
    {
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age))
            age--;
        return age;
    }
}
