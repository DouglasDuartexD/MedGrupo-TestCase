using Contacts.Application.Interfaces;
using Contacts.Application.UseCases;
using Contacts.Domain.Abstractions;
using Contacts.Domain.Entities;
using Contacts.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace Contacts.Tests.Application;

public class GetActiveContactByIdUseCaseTests
{
    private static readonly DateOnly FakeToday = new DateOnly(2026, 4, 10);
    private static readonly DateTime FakeUtcNow = new DateTime(2026, 4, 10, 12, 0, 0, DateTimeKind.Utc);

    private readonly IContactRepository _repository = Substitute.For<IContactRepository>();
    private readonly IDateTimeProvider _clock = Substitute.For<IDateTimeProvider>();
    private readonly GetActiveContactByIdUseCase _useCase;

    public GetActiveContactByIdUseCaseTests()
    {
        _clock.Today.Returns(FakeToday);
        _clock.UtcNow.Returns(FakeUtcNow);
        _useCase = new GetActiveContactByIdUseCase(_repository, _clock);
    }

    [Fact]
    public async Task Execute_ActiveContact_ShouldReturnResponse()
    {
        var contact = Contact.Create("Maria", FakeToday.AddYears(-30), Sex.Female, FakeToday, FakeUtcNow);
        _repository.GetActiveByIdAsync(contact.Id, default).Returns(contact);

        var result = await _useCase.ExecuteAsync(contact.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(contact.Id);
        result.IsActive.Should().BeTrue();
        result.Age.Should().Be(30);
    }

    [Fact]
    public async Task Execute_InactiveContact_ShouldReturnNull()
    {
        var id = Guid.NewGuid();
        _repository.GetActiveByIdAsync(id, default).Returns((Contact?)null);

        var result = await _useCase.ExecuteAsync(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Execute_NonExistentContact_ShouldReturnNull()
    {
        var id = Guid.NewGuid();
        _repository.GetActiveByIdAsync(id, default).Returns((Contact?)null);

        var result = await _useCase.ExecuteAsync(id);

        result.Should().BeNull();
    }
}
