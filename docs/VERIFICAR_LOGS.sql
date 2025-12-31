-- Verificar logs en ApplicationLogs

\echo '=== TOTAL DE LOGS ==='
SELECT COUNT(*) as total_logs FROM "ApplicationLogs";

\echo ''
\echo '=== ÚLTIMOS 10 LOGS ==='
SELECT 
    "Level",
    "Source",
    substring("Message", 1, 80) as message_preview,
    "Path",
    "CreatedAt"
FROM "ApplicationLogs"
ORDER BY "CreatedAt" DESC
LIMIT 10;

\echo ''
\echo '=== LOGS POR NIVEL ==='
SELECT 
    "Level",
    COUNT(*) as cantidad
FROM "ApplicationLogs"
GROUP BY "Level"
ORDER BY cantidad DESC;

\echo ''
\echo '=== LOGS DE ERROR Y WARNING ==='
SELECT 
    "Level",
    "Source",
    substring("Message", 1, 100) as message_preview,
    "ExceptionType",
    "Path",
    "CreatedAt"
FROM "ApplicationLogs"
WHERE "Level" IN ('Error', 'Critical', 'Warning')
ORDER BY "CreatedAt" DESC
LIMIT 10;

\echo ''
\echo '=== LOGS RELACIONADOS CON LOGIN ==='
SELECT 
    "Level",
    "Source",
    "Message",
    "ExceptionType",
    "Path",
    "CreatedAt",
    "RequestId"
FROM "ApplicationLogs"
WHERE "Path" LIKE '%Account/Login%' OR "Message" LIKE '%Login%'
ORDER BY "CreatedAt" DESC
LIMIT 10;

