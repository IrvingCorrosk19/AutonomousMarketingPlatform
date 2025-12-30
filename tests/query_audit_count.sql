SELECT COUNT(*) as total_logs, 
       COUNT(CASE WHEN "Action" = 'Login' THEN 1 END) as login_logs,
       COUNT(CASE WHEN "Result" = 'Success' THEN 1 END) as success_logs
FROM "AuditLogs";

