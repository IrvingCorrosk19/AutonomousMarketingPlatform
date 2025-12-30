using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Validators;
using FluentValidation;
using Xunit;

namespace AutonomousMarketingPlatform.Tests.Validators;

public class CreateCampaignDtoValidatorTests
{
    private readonly CreateCampaignDtoValidator _validator;

    public CreateCampaignDtoValidatorTests()
    {
        _validator = new CreateCampaignDtoValidator();
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = string.Empty,
            Status = "Draft"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && 
            e.ErrorMessage == "El nombre de la campaña es obligatorio.");
    }

    [Fact]
    public void Validate_WhenNameIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = null!,
            Status = "Draft"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WhenNameExceeds200Characters_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = new string('A', 201), // 201 caracteres
            Status = "Draft"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && 
            e.ErrorMessage == "El nombre no puede exceder 200 caracteres.");
    }

    [Fact]
    public void Validate_WhenNameIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test",
            Status = "Draft"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WhenDescriptionExceeds1000Characters_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test",
            Description = new string('A', 1001), // 1001 caracteres
            Status = "Draft"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description" && 
            e.ErrorMessage == "La descripción no puede exceder 1000 caracteres.");
    }

    [Fact]
    public void Validate_WhenStatusIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test",
            Status = "InvalidStatus"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Status" && 
            e.ErrorMessage == "El estado debe ser: Draft, Active, Paused o Archived.");
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("Active")]
    [InlineData("Paused")]
    [InlineData("Archived")]
    public void Validate_WhenStatusIsValid_ShouldNotHaveValidationError(string status)
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test",
            Status = status
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Status");
    }

    [Fact]
    public void Validate_WhenEndDateIsBeforeStartDate_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test",
            Status = "Draft",
            StartDate = new DateTime(2024, 12, 31),
            EndDate = new DateTime(2024, 12, 1)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate" && 
            e.ErrorMessage == "La fecha de fin debe ser posterior a la fecha de inicio.");
    }

    [Fact]
    public void Validate_WhenEndDateIsAfterStartDate_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test",
            Status = "Draft",
            StartDate = new DateTime(2024, 12, 1),
            EndDate = new DateTime(2024, 12, 31)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "EndDate");
    }

    [Fact]
    public void Validate_WhenBudgetIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test",
            Status = "Draft",
            Budget = -1000
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Budget" && 
            e.ErrorMessage == "El presupuesto no puede ser negativo.");
    }

    [Fact]
    public void Validate_WhenBudgetIsZero_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test",
            Status = "Draft",
            Budget = 0
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Budget");
    }

    [Fact]
    public void Validate_WhenBudgetIsPositive_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test",
            Status = "Draft",
            Budget = 50000
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Budget");
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldPassValidation()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            Name = "Campaña Test 2024",
            Description = "Descripción válida",
            Status = "Active",
            StartDate = new DateTime(2024, 12, 1),
            EndDate = new DateTime(2024, 12, 31),
            Budget = 50000
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}

