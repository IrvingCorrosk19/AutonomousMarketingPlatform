using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Validators;
using FluentValidation;
using Xunit;

namespace AutonomousMarketingPlatform.Tests.Validators;

public class RegisterCampaignMetricsDtoValidatorTests
{
    private readonly RegisterCampaignMetricsDtoValidator _validator;

    public RegisterCampaignMetricsDtoValidatorTests()
    {
        _validator = new RegisterCampaignMetricsDtoValidator();
    }

    [Fact]
    public void Validate_WhenCampaignIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.Empty,
            MetricDate = DateTime.UtcNow.Date
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CampaignId" && 
            e.ErrorMessage == "El ID de la campaña es obligatorio.");
    }

    [Fact]
    public void Validate_WhenMetricDateIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = default(DateTime)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MetricDate" && 
            e.ErrorMessage == "La fecha de las métricas es obligatoria.");
    }

    [Fact]
    public void Validate_WhenMetricDateIsFuture_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date.AddDays(2) // 2 días en el futuro
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MetricDate" && 
            e.ErrorMessage == "La fecha no puede ser futura.");
    }

    [Fact]
    public void Validate_WhenMetricDateIsToday_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "MetricDate");
    }

    [Fact]
    public void Validate_WhenMetricDateIsTomorrow_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date.AddDays(1) // Mañana está permitido
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "MetricDate");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WhenImpressionsIsNegative_ShouldHaveValidationError(int impressions)
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Impressions = impressions
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Impressions" && 
            e.ErrorMessage == "Las impresiones no pueden ser negativas.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(1000000)]
    public void Validate_WhenImpressionsIsNonNegative_ShouldNotHaveValidationError(int impressions)
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Impressions = impressions
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Impressions");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-50)]
    public void Validate_WhenClicksIsNegative_ShouldHaveValidationError(int clicks)
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Clicks = clicks
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Clicks" && 
            e.ErrorMessage == "Los clics no pueden ser negativos.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_WhenLikesIsNegative_ShouldHaveValidationError(int likes)
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Likes = likes
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Likes" && 
            e.ErrorMessage == "Los likes no pueden ser negativos.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Validate_WhenCommentsIsNegative_ShouldHaveValidationError(int comments)
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Comments = comments
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Comments" && 
            e.ErrorMessage == "Los comentarios no pueden ser negativos.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-20)]
    public void Validate_WhenSharesIsNegative_ShouldHaveValidationError(int shares)
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Shares = shares
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Shares" && 
            e.ErrorMessage == "Los compartidos no pueden ser negativos.");
    }

    [Fact]
    public void Validate_WhenSourceExceeds50Characters_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Source = new string('A', 51) // 51 caracteres
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Source" && 
            e.ErrorMessage == "La fuente no puede exceder 50 caracteres.");
    }

    [Fact]
    public void Validate_WhenNotesExceeds2000Characters_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Notes = new string('A', 2001) // 2001 caracteres
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Notes" && 
            e.ErrorMessage == "Las notas no pueden exceder 2000 caracteres.");
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldPassValidation()
    {
        // Arrange
        var dto = new RegisterCampaignMetricsDto
        {
            CampaignId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Impressions = 10000,
            Clicks = 500,
            Likes = 200,
            Comments = 50,
            Shares = 30,
            Source = "Facebook",
            Notes = "Métricas del día"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}


