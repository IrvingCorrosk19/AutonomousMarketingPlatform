using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Threading.Tasks;

namespace AutonomousMarketingPlatform.Tests;

/// <summary>
/// Clase para ejecutar pruebas de autenticación y verificar en BD.
/// </summary>
public class AuthTestRunner
{
    private readonly string _connectionString;

    public AuthTestRunner(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Obtiene información de un usuario desde la BD.
    /// </summary>
    public async Task<Dictionary<string, object?>> GetUserInfoAsync(string email)
    {
        var userInfo = new Dictionary<string, object?>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT 
                id, 
                email, 
                ""FullName"", 
                ""TenantId"", 
                ""IsActive"", 
                ""FailedLoginAttempts"", 
                ""LockoutEndDate"", 
                ""LastLoginAt"", 
                ""LastLoginIp""
            FROM ""AspNetUsers""
            WHERE email = @email";

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("email", email);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            userInfo["Id"] = reader.GetGuid(0);
            userInfo["Email"] = reader.GetString(1);
            userInfo["FullName"] = reader.IsDBNull(2) ? null : reader.GetString(2);
            userInfo["TenantId"] = reader.GetGuid(3);
            userInfo["IsActive"] = reader.GetBoolean(4);
            userInfo["FailedLoginAttempts"] = reader.GetInt32(5);
            userInfo["LockoutEndDate"] = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
            userInfo["LastLoginAt"] = reader.IsDBNull(7) ? null : reader.GetDateTime(7);
            userInfo["LastLoginIp"] = reader.IsDBNull(8) ? null : reader.GetString(8);
        }

        return userInfo;
    }

    /// <summary>
    /// Obtiene eventos de auditoría relacionados con login.
    /// </summary>
    public async Task<List<Dictionary<string, object?>>> GetLoginAuditLogsAsync(Guid? userId = null, int limit = 10)
    {
        var logs = new List<Dictionary<string, object?>>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT 
                ""Id"", 
                ""TenantId"", 
                ""UserId"", 
                ""Action"", 
                ""Status"", 
                ""Message"", 
                ""IpAddress"", 
                ""CreatedAt""
            FROM ""AuditLogs""
            WHERE ""Action"" = 'Login'";

        if (userId.HasValue)
        {
            query += " AND \"UserId\" = @userId";
        }

        query += " ORDER BY \"CreatedAt\" DESC LIMIT @limit";

        await using var command = new NpgsqlCommand(query, connection);
        if (userId.HasValue)
        {
            command.Parameters.AddWithValue("userId", userId.Value);
        }
        command.Parameters.AddWithValue("limit", limit);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var log = new Dictionary<string, object?>
            {
                ["Id"] = reader.GetGuid(0),
                ["TenantId"] = reader.GetGuid(1),
                ["UserId"] = reader.GetGuid(2),
                ["Action"] = reader.GetString(3),
                ["Status"] = reader.GetString(4),
                ["Message"] = reader.IsDBNull(5) ? null : reader.GetString(5),
                ["IpAddress"] = reader.IsDBNull(6) ? null : reader.GetString(6),
                ["CreatedAt"] = reader.GetDateTime(7)
            };
            logs.Add(log);
        }

        return logs;
    }

    /// <summary>
    /// Resetea los intentos fallidos de un usuario (útil para pruebas).
    /// </summary>
    public async Task ResetUserFailedAttemptsAsync(string email)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            UPDATE ""AspNetUsers""
            SET ""FailedLoginAttempts"" = 0, ""LockoutEndDate"" = NULL
            WHERE email = @email";

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("email", email);

        await command.ExecuteNonQueryAsync();
    }
}

