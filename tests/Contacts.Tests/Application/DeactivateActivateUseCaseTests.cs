using Contacts.Application.Interfaces;
using Contacts.Application.UseCases;
using Contacts.Domain.Abstractions;
using Contacts.Domain.Entities;
using Contacts.Domain.Enums;
using Contacts.Domain.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace Contacts.Tests.Application;

public class DeactivateActivateUseCaseTests
{
    private static readonly DateOnly FakeToday = new DateOnly(2026, 4, 10);
    private static readonly DateTime FakeUtcNow = new DateTime(2026, 4, 10, 12, 0, 0, DateTimeKind.Utc);

    private readonly IContactRepository _repository = Substitute.For<IContactRepository>();
    private readonly IDateTimeProvider _clock = Substitute.For<IDateTimeProvider>();
    private readonly DeactivateContactUseCase _deactivateUseCase;
    private readonly ActivateContactUseCase _activateUseCase;

    public DeactivateActivateUseCaseTests()
    {
        _clock.Today.Returns(FakeToday);
        _clock.UtcNow.Returns(FakeUtcNow);
        _deactivateUseCase = new DeactivateContactUseCase(_repository, _clock);
        _activateUseCase = new ActivateContactUseCase(_repository, _clock);
    }

    private Contact CreateActiveContact() =>
        Contact.Create("Fulano", FakeToday.AddYears(-30), Sex.Male, FakeToday, FakeUtcNow);

    [Fact]
    public async Task Deactivate_ActiveContact_ShouldSetInactive()
    {
        var contact = CreateActiveContact();
        _repository.GetByIdAsync(contact.Id, default).Returns(contact);

        var result = await _deactivateUseCase.ExecuteAsync(contact.Id);

        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();
        await _repository.Received(1).UpdateAsync(contact, default);
    }

    [Fact]
    public async Task Deactivate_AlreadyInactiveContact_ShouldThrowConflictException()
    {
        var contact = CreateActiveContact();
        contact.Deactivate(FakeUtcNow);
        _repository.GetByIdAsync(contact.Id, default).Returns(contact);

        var act = () => _deactivateUseCase.ExecuteAsync(contact.Id);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already inactive*");
    }

    [Fact]
    public async Task Deactivate_NonExistentContact_ShouldReturnNull()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, default).Returns((Contact?)null);

        var result = await _deactivateUseCase.ExecuteAsync(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Activate_InactiveContact_ShouldSetActive()
    {
        var contact = CreateActiveContact();
        contact.Deactivate(FakeUtcNow);
        _repository.GetByIdAsync(contact.Id, default).Returns(contact);

        var result = await _activateUseCase.ExecuteAsync(contact.Id);

        result.Should().NotBeNull();
        result!.IsActive.Should().BeTrue();
        await _repository.Received(1).UpdateAsync(contact, default);
    }

    [Fact]
    public async Task Activate_AlreadyActiveContact_ShouldThrowConflictException()
    {
        var contact = CreateActiveContact();
        _repository.GetByIdAsync(contact.Id, default).Returns(contact);

        var act = () => _activateUseCase.ExecuteAsync(contact.Id);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already active*");
    }

    [Fact]
    public async Task Activate_NonExistentContact_ShouldReturnNull()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, default).Returns((Contact?)null);

        var result = await _activateUseCase.ExecuteAsync(id);

        result.Should().BeNull();
    }
}
