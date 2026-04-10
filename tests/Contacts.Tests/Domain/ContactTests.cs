using Contacts.Domain.Entities;
using Contacts.Domain.Enums;
using Contacts.Domain.Exceptions;
using FluentAssertions;

namespace Contacts.Tests.Domain;

public class ContactTests
{
    private static readonly DateOnly Today = new DateOnly(2026, 4, 10);
    private static readonly DateTime UtcNow = new DateTime(2026, 4, 10, 12, 0, 0, DateTimeKind.Utc);

    private static DateOnly AdultBirthDate() => Today.AddYears(-30);

    private static Contact CreateAdult(string name = "Maria Silva") =>
        Contact.Create(name, AdultBirthDate(), Sex.Female, Today, UtcNow);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_ValidData_ShouldSucceed()
    {
        var contact = CreateAdult();

        contact.Name.Should().Be("Maria Silva");
        contact.IsActive.Should().BeTrue();
        contact.CreatedAt.Should().Be(UtcNow);
        contact.UpdatedAt.Should().Be(UtcNow);
    }

    [Fact]
    public void Create_FutureBirthDate_ShouldThrowDomainException()
    {
        var futureBirthDate = Today.AddDays(1);

        var act = () => Contact.Create("Joao", futureBirthDate, Sex.Male, Today, UtcNow);

        act.Should().Throw<DomainException>()
            .Which.Errors.Should().Contain("BirthDate cannot be greater than today.");
    }

    [Fact]
    public void Create_MinorAge_ShouldThrowDomainException()
    {
        var minorBirthDate = Today.AddYears(-17);

        var act = () => Contact.Create("Joao", minorBirthDate, Sex.Male, Today, UtcNow);

        act.Should().Throw<DomainException>()
            .Which.Errors.Should().Contain("Contact must be 18 years old or older.");
    }

    [Fact]
    public void Create_EmptyName_ShouldThrowDomainException()
    {
        var act = () => Contact.Create("", AdultBirthDate(), Sex.Male, Today, UtcNow);

        act.Should().Throw<DomainException>()
            .Which.Errors.Should().Contain("Name is required.");
    }

    [Fact]
    public void Create_ExactlyZeroAge_ShouldThrowDomainException()
    {
        var act = () => Contact.Create("Bebe", Today, Sex.NotInformed, Today, UtcNow);

        act.Should().Throw<DomainException>()
            .Which.Errors.Should().Contain("Age cannot be zero.");
    }

    [Fact]
    public void Create_SetsCreatedAtAndUpdatedAt_FromProvidedUtcNow()
    {
        var specificTime = new DateTime(2025, 6, 15, 8, 30, 0, DateTimeKind.Utc);
        var contact = Contact.Create("Ana", AdultBirthDate(), Sex.Female, Today, specificTime);

        contact.CreatedAt.Should().Be(specificTime);
        contact.UpdatedAt.Should().Be(specificTime);
    }

    // ── Age calculation ───────────────────────────────────────────────────────

    [Fact]
    public void CalculateAge_BirthdayNotYetOccurredThisYear_ShouldSubtractOne()
    {
        var tomorrow = Today.AddDays(1);
        var birthDate = new DateOnly(Today.Year - 30, tomorrow.Month, tomorrow.Day);

        var age = Contact.CalculateAge(birthDate, Today);

        age.Should().Be(29);
    }

    [Fact]
    public void CalculateAge_BirthdayAlreadyOccurredThisYear_ShouldBeCorrect()
    {
        var yesterday = Today.AddDays(-1);
        var birthDate = new DateOnly(Today.Year - 30, yesterday.Month, yesterday.Day);

        var age = Contact.CalculateAge(birthDate, Today);

        age.Should().Be(30);
    }

    [Fact]
    public void CalculateAge_UsesProvidedTodayNotSystemClock()
    {
        var birthDate = new DateOnly(1990, 4, 10);
        var referenceDateA = new DateOnly(2026, 4, 9);  // um dia antes do aniversário
        var referenceDateB = new DateOnly(2026, 4, 10); // exatamente no aniversário

        Contact.CalculateAge(birthDate, referenceDateA).Should().Be(35);
        Contact.CalculateAge(birthDate, referenceDateB).Should().Be(36);
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_ValidData_ShouldUpdateFields()
    {
        var contact = CreateAdult("Joao Antigo");
        var newBirthDate = Today.AddYears(-25);
        var updateTime = UtcNow.AddHours(1);

        contact.Update("Joao Novo", newBirthDate, Sex.Other, Today, updateTime);

        contact.Name.Should().Be("Joao Novo");
        contact.BirthDate.Should().Be(newBirthDate);
        contact.Sex.Should().Be(Sex.Other);
        contact.UpdatedAt.Should().Be(updateTime);
    }

    [Fact]
    public void Update_MinorAge_ShouldThrowDomainException()
    {
        var contact = CreateAdult("Fulano");
        var minorBirthDate = Today.AddYears(-10);

        var act = () => contact.Update("Fulano", minorBirthDate, Sex.Male, Today, UtcNow);

        act.Should().Throw<DomainException>()
            .Which.Errors.Should().Contain("Contact must be 18 years old or older.");
    }

    // ── Deactivate / Activate ─────────────────────────────────────────────────

    [Fact]
    public void Deactivate_ActiveContact_ShouldSetIsActiveFalse()
    {
        var contact = CreateAdult();
        var deactivateTime = UtcNow.AddHours(2);

        contact.Deactivate(deactivateTime);

        contact.IsActive.Should().BeFalse();
        contact.UpdatedAt.Should().Be(deactivateTime);
    }

    [Fact]
    public void Activate_InactiveContact_ShouldSetIsActiveTrue()
    {
        var contact = CreateAdult();
        contact.Deactivate(UtcNow);
        var activateTime = UtcNow.AddHours(3);

        contact.Activate(activateTime);

        contact.IsActive.Should().BeTrue();
        contact.UpdatedAt.Should().Be(activateTime);
    }
}
