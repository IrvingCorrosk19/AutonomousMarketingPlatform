using System.Security.Claims;
using AutonomousMarketingPlatform.Web.Helpers;
using Xunit;

namespace AutonomousMarketingPlatform.Tests.Helpers;

public class UserHelperTests
{
    [Fact]
    public void GetUserId_WhenUserIdClaimExists_ShouldReturnGuid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetUserId(principal);

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserId_WhenUserIdClaimIsMissing_ShouldReturnNull()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetUserId(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserId_WhenUserIdClaimIsInvalid_ShouldReturnNull()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetUserId(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetTenantId_WhenTenantIdClaimExists_ShouldReturnGuid()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim("TenantId", tenantId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetTenantId(principal);

        // Assert
        Assert.Equal(tenantId, result);
    }

    [Fact]
    public void GetTenantId_WhenTenantIdClaimIsMissing_ShouldReturnNull()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetTenantId(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFullName_WhenFullNameClaimExists_ShouldReturnFullName()
    {
        // Arrange
        var fullName = "Juan Pérez";
        var claims = new List<Claim>
        {
            new Claim("FullName", fullName)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetFullName(principal);

        // Assert
        Assert.Equal(fullName, result);
    }

    [Fact]
    public void GetFullName_WhenFullNameClaimIsMissingButNameExists_ShouldReturnName()
    {
        // Arrange
        var name = "Juan Pérez";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, name)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetFullName(principal);

        // Assert
        Assert.Equal(name, result);
    }

    [Fact]
    public void GetEmail_WhenEmailClaimExists_ShouldReturnEmail()
    {
        // Arrange
        var email = "test@example.com";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetEmail(principal);

        // Assert
        Assert.Equal(email, result);
    }

    [Fact]
    public void GetEmail_WhenEmailClaimIsMissing_ShouldReturnNull()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetEmail(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void HasRole_WhenUserHasRole_ShouldReturnTrue()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "Owner")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.HasRole(principal, "Admin");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasRole_WhenUserDoesNotHaveRole_ShouldReturnFalse()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Marketer")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.HasRole(principal, "Admin");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetRoles_WhenUserHasMultipleRoles_ShouldReturnAllRoles()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "Owner"),
            new Claim(ClaimTypes.Role, "Marketer")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetRoles(principal).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains("Admin", result);
        Assert.Contains("Owner", result);
        Assert.Contains("Marketer", result);
    }

    [Fact]
    public void GetRoles_WhenUserHasNoRoles_ShouldReturnEmpty()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = UserHelper.GetRoles(principal).ToList();

        // Assert
        Assert.Empty(result);
    }
}


