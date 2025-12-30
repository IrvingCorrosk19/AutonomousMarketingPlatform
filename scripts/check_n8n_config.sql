-- Verificar configuración de n8n
SELECT 
    "Id",
    "TenantId",
    "UseMock",
    "BaseUrl",
    "ApiUrl",
    "DefaultWebhookUrl",
    "WebhookUrlsJson",
    "IsActive"
FROM "TenantN8nConfigs";

