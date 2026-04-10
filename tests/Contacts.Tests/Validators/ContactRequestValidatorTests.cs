using Contacts.Application.DTOs;
using Contacts.Application.Validators;
using Contacts.Domain.Enums;
using Contacts.Infrastructure.Services;
using FluentAssertions;

namespace Contacts.Tests.Validators;

public class ContactRequestValidatorTests
{
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
    private readonly ContactRequestValidator _validator = new(new DateTimeProvider());

    private static ContactRequest ValidRequest() => new()
    {
        Name = "Maria Silva",
        BirthDate = Today.AddYears(-30),
        Sex = Sex.Female
    };

    // ── Name ──────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Validate_ValidRequest_ShouldPass()
    {
        var result = await _validator.ValidateAsync(ValidRequest());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyName_ShouldFail()
    {
        var request = ValidRequest();
        request.Name = "";

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name is required.");
    }

    [Fact]
    public async Task Validate_WhitespaceName_ShouldFail()
    {
        var request = ValidRequest();
        request.Name = "   ";

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name is required.");
    }

    [Fact]
    public async Task Validate_NameExceeds150Chars_ShouldFail()
    {
        var request = ValidRequest();
        request.Name = new string('A', 151);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name must not exceed 150 characters.");
    }

    // ── BirthDate ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Validate_DefaultBirthDate_ShouldFail()
    {
        var request = ValidRequest();
        request.BirthDate = DateOnly.MinValue;

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "BirthDate is required.");
    }

    [Fact]
    public async Task Validate_FutureBirthDate_ShouldFail()
    {
        var request = ValidRequest();
        request.BirthDate = Today.AddDays(1);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "BirthDate cannot be greater than today.");
    }

    [Fact]
    public async Task Validate_BirthDateToday_ShouldFailAgeZero()
    {
        var request = ValidRequest();
        request.BirthDate = Today;

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Age cannot be zero.");
    }

    [Fact]
    public async Task Validate_MinorAge_ShouldFail()
    {
        var request = ValidRequest();
        request.BirthDate = Today.AddYears(-17);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Contact must be 18 years old or older.");
    }

    [Fact]
    public async Task Validate_Exactly18YearsOld_ShouldPass()
    {
        var request = ValidRequest();
        request.BirthDate = Today.AddYears(-18);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    // ── Sex ───────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Validate_NullSex_ShouldFail()
    {
        var request = ValidRequest();
        request.Sex = null;

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Sex is required.");
    }

    [Fact]
    public async Task Validate_InvalidSexValue_ShouldFail()
    {
        var request = ValidRequest();
        request.Sex = (Sex)99;

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Sex has an invalid value.");
    }

    [Theory]
    [InlineData(Sex.Female)]
    [InlineData(Sex.Male)]
    [InlineData(Sex.Other)]
    [InlineData(Sex.NotInformed)]
    public async Task Validate_AllValidSexValues_ShouldPass(Sex sex)
    {
        var request = ValidRequest();
        request.Sex = sex;

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }
}
