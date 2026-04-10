namespace Contacts.Domain.Exceptions;

/// <summary>
/// Representa uma violação de regras de domínio ou de validação de negócio.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Lista de erros associados à falha de domínio.
    /// </summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>
    /// Inicializa a exceção com uma única mensagem de erro.
    /// </summary>
    public DomainException(string error) : base(error)
    {
        Errors = new List<string> { error };
    }

    /// <summary>
    /// Inicializa a exceção com múltiplas mensagens de erro.
    /// </summary>
    public DomainException(IEnumerable<string> errors) : base(string.Join("; ", errors))
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
