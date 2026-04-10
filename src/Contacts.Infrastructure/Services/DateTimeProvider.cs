using Contacts.Domain.Abstractions;

namespace Contacts.Infrastructure.Services;

/// <summary>
/// Implementação padrão de relógio baseada no tempo UTC do sistema.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    public DateTime UtcNow => DateTime.UtcNow;
}
