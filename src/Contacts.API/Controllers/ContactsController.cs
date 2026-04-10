using Contacts.Application.DTOs;
using Contacts.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.API.Controllers;

[ApiController]
[Route("api/contacts")]
[Produces("application/json")]
/// <summary>
/// Expõe via HTTP as operações de gerenciamento de contatos.
/// </summary>
public class ContactsController : ControllerBase
{
    private readonly CreateContactUseCase _createContact;
    private readonly ListActiveContactsUseCase _listActiveContacts;
    private readonly GetActiveContactByIdUseCase _getActiveContactById;
    private readonly UpdateContactUseCase _updateContact;
    private readonly DeactivateContactUseCase _deactivateContact;
    private readonly ActivateContactUseCase _activateContact;
    private readonly DeleteContactUseCase _deleteContact;

    /// <summary>
    /// Inicializa o controller com os casos de uso necessários para cada operação.
    /// </summary>
    public ContactsController(
        CreateContactUseCase createContact,
        ListActiveContactsUseCase listActiveContacts,
        GetActiveContactByIdUseCase getActiveContactById,
        UpdateContactUseCase updateContact,
        DeactivateContactUseCase deactivateContact,
        ActivateContactUseCase activateContact,
        DeleteContactUseCase deleteContact)
    {
        _createContact = createContact;
        _listActiveContacts = listActiveContacts;
        _getActiveContactById = getActiveContactById;
        _updateContact = updateContact;
        _deactivateContact = deactivateContact;
        _activateContact = activateContact;
        _deleteContact = deleteContact;
    }

    /// <summary>
    /// Cria um novo contato.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ContactRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
            return BadRequest(new { type = "validation_error", errors = new[] { "Request body is required or contains malformed data." } });

        var result = await _createContact.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Lista todos os contatos ativos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ContactResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAll(CancellationToken cancellationToken)
    {
        var result = await _listActiveContacts.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retorna os detalhes de um contato ativo.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getActiveContactById.ExecuteAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Atualiza os dados de um contato ativo.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContactRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
            return BadRequest(new { type = "validation_error", errors = new[] { "Request body is required or contains malformed data." } });

        var result = await _updateContact.ExecuteAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Desativa um contato ativo.
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deactivateContact.ExecuteAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Ativa um contato inativo.
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _activateContact.ExecuteAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Exclui fisicamente um contato.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deleteContact.ExecuteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
