using AutoFixture;
using FluentAssertions;
using Challenge02.Domain.Models;
using Challenge02.Domain.Services;

public class EmailNormalizationServiceTests
{
    private readonly IFixture _fixture;
    private readonly EmailNormalizationService _service;

    public EmailNormalizationServiceTests()
    {
        _fixture = new Fixture();
        _service = new EmailNormalizationService();
    }

    [Fact]
    public void TryNormalizeEmailDomain_WhenValidEmail_ShouldChangesDomain()
    {
        var dev = _fixture.Build<Dev>()
            .With(d => d.Email, "example@outrodominio.com")
            .Create();

        var result = _service.TryNormalizeEmailDomain(dev);

        result.Should().BeTrue();
        dev.Email.Should().Be("example@challenge.com.br");
    }

    [Fact]
    public void TryNormalizeEmailDomain_WhenEmailWithchallengeDomain_ReturnsFalseAndDoesNotChangeEmail()
    {
        var expectedEmail = "example@challenge.com.br";
        var dev = _fixture.Build<Dev>()
            .With(d => d.Email, expectedEmail)
            .Create();

        var result = _service.TryNormalizeEmailDomain(dev);

        result.Should().BeFalse();
        dev.Email.Should().Be(expectedEmail);
    }

    [Fact]
    public void TryNormalizeEmailDomain_WhenNullEmail_ReturnsFalse()
    {
        var dev = _fixture.Build<Dev>()
            .With(d => d.Email, (string)null)
            .Create();

        var result = _service.TryNormalizeEmailDomain(dev);

        result.Should().BeFalse();
    }

}
