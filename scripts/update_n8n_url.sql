-- Actualizar configuración de n8n con la URL correcta
UPDATE "TenantN8nConfigs"
SET 
    "BaseUrl" = 'https://n8n.bashpty.com',
    "ApiUrl" = 'https://n8n.bashpty.com/api/v1',
    "DefaultWebhookUrl" = 'https://n8n.bashpty.com/webhook',
    "WebhookUrlsJson" = jsonb_set(
        COALESCE("WebhookUrlsJson"::jsonb, '{}'::jsonb),
        '{MarketingRequest}',
        '"https://n8n.bashpty.com/webhook/marketing-request"'
    )::text,
    "UpdatedAt" = NOW()
WHERE "TenantId" = 'eabb1423-ca22-4f96-817d-d068a5c5fd5f';

-- Verificar la configuración actualizada
SELECT 
    "BaseUrl",
    "ApiUrl",
    "DefaultWebhookUrl",
    "WebhookUrlsJson"
FROM "TenantN8nConfigs"
WHERE "TenantId" = 'eabb1423-ca22-4f96-817d-d068a5c5fd5f';

