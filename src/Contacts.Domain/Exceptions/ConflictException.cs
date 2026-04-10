namespace Contacts.Domain.Exceptions;

/// <summary>
/// Representa uma operação inválida para o estado atual do recurso.
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Mensagem detalhando o conflito encontrado.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Inicializa a exceção com a descrição do conflito.
    /// </summary>
    public ConflictException(string error) : base(error)
    {
        Error = error;
    }
}
