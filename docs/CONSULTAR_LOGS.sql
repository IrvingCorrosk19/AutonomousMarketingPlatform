-- Consultar logs de ApplicationLogs

-- 1. Contar total de logs
SELECT COUNT(*) as total_logs FROM "ApplicationLogs";

-- 2. Ver logs recientes (últimos 20)
SELECT 
    "Level",
    "Source",
    LEFT("Message", 100) as message_preview,
    "Path",
    "CreatedAt",
    "RequestId"
FROM "ApplicationLogs"
ORDER BY "CreatedAt" DESC
LIMIT 20;

-- 3. Estadísticas por nivel
SELECT 
    "Level",
    COUNT(*) as cantidad
FROM "ApplicationLogs"
GROUP BY "Level"
ORDER BY cantidad DESC;

-- 4. Logs de error y warning recientes
SELECT 
    "Level",
    "Source",
    LEFT("Message", 150) as message_preview,
    "ExceptionType",
    "Path",
    "CreatedAt"
FROM "ApplicationLogs"
WHERE "Level" IN ('Error', 'Critical', 'Warning')
ORDER BY "CreatedAt" DESC
LIMIT 10;

-- 5. Logs relacionados con /Account/Login
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
LIMIT 20;

