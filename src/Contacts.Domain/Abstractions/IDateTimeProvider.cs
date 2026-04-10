namespace Contacts.Domain.Abstractions;

/// <summary>
/// Fornece a data atual e o timestamp UTC usados pelas regras de negócio.
/// </summary>
public interface IDateTimeProvider
{
    DateOnly Today { get; }
    DateTime UtcNow { get; }
}
