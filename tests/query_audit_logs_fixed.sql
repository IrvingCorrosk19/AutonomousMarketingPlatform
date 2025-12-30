SELECT "Id", "TenantId", "UserId", "Action", "Status", "Message", "IpAddress", "CreatedAt" 
FROM "AuditLogs" 
WHERE "Action" = 'Login' 
ORDER BY "CreatedAt" DESC 
LIMIT 5;

