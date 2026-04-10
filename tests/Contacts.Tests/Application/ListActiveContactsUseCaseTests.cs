using Contacts.Application.Interfaces;
using Contacts.Application.UseCases;
using Contacts.Domain.Abstractions;
using Contacts.Domain.Entities;
using Contacts.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace Contacts.Tests.Application;

public class ListActiveContactsUseCaseTests
{
    private static readonly DateOnly FakeToday = new DateOnly(2026, 4, 10);
    private static readonly DateTime FakeUtcNow = new DateTime(2026, 4, 10, 12, 0, 0, DateTimeKind.Utc);

    private readonly IContactRepository _repository = Substitute.For<IContactRepository>();
    private readonly IDateTimeProvider _clock = Substitute.For<IDateTimeProvider>();
    private readonly ListActiveContactsUseCase _useCase;

    public ListActiveContactsUseCaseTests()
    {
        _clock.Today.Returns(FakeToday);
        _clock.UtcNow.Returns(FakeUtcNow);
        _useCase = new ListActiveContactsUseCase(_repository, _clock);
    }

    [Fact]
    public async Task Execute_ShouldReturnOnlyActiveContacts()
    {
        var activeContact = Contact.Create("Ativo", FakeToday.AddYears(-25), Sex.Female, FakeToday, FakeUtcNow);
        _repository.ListActiveAsync(default).Returns(new List<Contact> { activeContact }.AsReadOnly());

        var result = await _useCase.ExecuteAsync();

        result.Should().HaveCount(1);
        result[0].IsActive.Should().BeTrue();
        result[0].Age.Should().Be(25);
    }

    [Fact]
    public async Task Execute_NoActiveContacts_ShouldReturnEmptyList()
    {
        _repository.ListActiveAsync(default).Returns(new List<Contact>().AsReadOnly());

        var result = await _useCase.ExecuteAsync();

        result.Should().BeEmpty();
    }
}
