using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace Contacts.Tests.Integration;

public class ContactsEndpointValidationTests : IClassFixture<TestWebApplicationFactory>
{
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
    private static string AdultBirthDate => Today.AddYears(-35).ToString("yyyy-MM-dd");
    private static string FutureBirthDate => Today.AddDays(1).ToString("yyyy-MM-dd");
    private static string MinorBirthDate => Today.AddYears(-17).ToString("yyyy-MM-dd");

    private readonly HttpClient _client;

    public ContactsEndpointValidationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // ── POST /api/contacts ────────────────────────────────────────────────────

    [Fact]
    public async Task Post_ValidContact_Returns201()
    {
        var payload = new { name = "Maria Silva", birthDate = AdultBirthDate, sex = "Female" };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_MissingName_Returns400WithValidationError()
    {
        var payload = new { birthDate = AdultBirthDate, sex = "Female" };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);
        var body = await ParseError(response);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Type.Should().Be("validation_error");
        body.Errors.Should().Contain("Name is required.");
    }

    [Fact]
    public async Task Post_MissingBirthDate_Returns400()
    {
        var payload = new { name = "Joao", sex = "Male" };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);
        var body = await ParseError(response);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Type.Should().Be("validation_error");
        body.Errors.Should().Contain("BirthDate is required.");
    }

    [Fact]
    public async Task Post_DefaultBirthDate_Returns400()
    {
        var payload = new { name = "Joao", birthDate = "0001-01-01", sex = "Male" };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);
        var body = await ParseError(response);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Type.Should().Be("validation_error");
        body.Errors.Should().Contain("BirthDate is required.");
    }

    [Fact]
    public async Task Post_FutureBirthDate_Returns400()
    {
        var payload = new { name = "Joao", birthDate = FutureBirthDate, sex = "Male" };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);
        var body = await ParseError(response);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Type.Should().Be("validation_error");
        body.Errors.Should().Contain("BirthDate cannot be greater than today.");
    }

    [Fact]
    public async Task Post_MinorAge_Returns400()
    {
        var payload = new { name = "Jovem", birthDate = MinorBirthDate, sex = "Female" };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);
        var body = await ParseError(response);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Type.Should().Be("validation_error");
        body.Errors.Should().Contain("Contact must be 18 years old or older.");
    }

    [Fact]
    public async Task Post_InvalidSex_Returns400()
    {
        var payload = new { name = "Joao", birthDate = AdultBirthDate, sex = "Extraterrestre" };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);

        // JSON inválido para enum → 400 (seja por binding ou por validação)
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_MissingSex_Returns400WithValidationError()
    {
        var payload = new { name = "Joao", birthDate = AdultBirthDate };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);
        var body = await ParseError(response);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Type.Should().Be("validation_error");
        body.Errors.Should().Contain("Sex is required.");
    }

    [Fact]
    public async Task Post_IntegerSexZero_Returns400()
    {
        var payload = new { name = "Joao", birthDate = AdultBirthDate, sex = 0 };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_IntegerSexInvalid_Returns400()
    {
        var payload = new { name = "Joao", birthDate = AdultBirthDate, sex = 99 };

        var response = await _client.PostAsJsonAsync("/api/contacts", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── PUT /api/contacts/{id} ────────────────────────────────────────────────

    [Fact]
    public async Task Put_InvalidPayload_Returns400WithValidationError()
    {
        var id = Guid.NewGuid();
        var payload = new { name = "", birthDate = AdultBirthDate, sex = "Male" };

        var response = await _client.PutAsJsonAsync($"/api/contacts/{id}", payload);
        var body = await ParseError(response);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Type.Should().Be("validation_error");
        body.Errors.Should().Contain("Name is required.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async Task<ErrorResponse> ParseError(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ErrorResponse>(content, JsonOptions)
               ?? throw new InvalidOperationException($"Could not parse error body: {content}");
    }

    private record ErrorResponse(string Type, List<string> Errors);
}
