using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Validators;
using FluentValidation;
using Xunit;

namespace AutonomousMarketingPlatform.Tests.Validators;

public class RegisterPublishingJobMetricsDtoValidatorTests
{
    private readonly RegisterPublishingJobMetricsDtoValidator _validator;

    public RegisterPublishingJobMetricsDtoValidatorTests()
    {
        _validator = new RegisterPublishingJobMetricsDtoValidator();
    }

    [Fact]
    public void Validate_WhenPublishingJobIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.Empty,
            MetricDate = DateTime.UtcNow.Date
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PublishingJobId" && 
            e.ErrorMessage == "El ID de la publicación es obligatorio.");
    }

    [Fact]
    public void Validate_WhenMetricDateIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
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
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date.AddDays(2)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MetricDate" && 
            e.ErrorMessage == "La fecha no puede ser futura.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WhenImpressionsIsNegative_ShouldHaveValidationError(long impressions)
    {
        // Arrange
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
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
    [InlineData(-1)]
    [InlineData(-50)]
    public void Validate_WhenClicksIsNegative_ShouldHaveValidationError(long clicks)
    {
        // Arrange
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
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
    public void Validate_WhenLikesIsNegative_ShouldHaveValidationError(long likes)
    {
        // Arrange
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
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
    public void Validate_WhenCommentsIsNegative_ShouldHaveValidationError(long comments)
    {
        // Arrange
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
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
    public void Validate_WhenSharesIsNegative_ShouldHaveValidationError(long shares)
    {
        // Arrange
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
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
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Source = new string('A', 51)
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
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Notes = new string('A', 2001)
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
        var dto = new RegisterPublishingJobMetricsDto
        {
            PublishingJobId = Guid.NewGuid(),
            MetricDate = DateTime.UtcNow.Date,
            Impressions = 5000,
            Clicks = 250,
            Likes = 100,
            Comments = 25,
            Shares = 15,
            Source = "Instagram",
            Notes = "Métricas del post"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}


