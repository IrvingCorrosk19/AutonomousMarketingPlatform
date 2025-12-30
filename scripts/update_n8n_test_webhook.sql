-- Actualizar URL del webhook de Marketing Request a la URL de TEST
UPDATE "TenantN8nConfigs"
SET 
    "WebhookUrlsJson" = jsonb_set(
        COALESCE("WebhookUrlsJson"::jsonb, '{}'::jsonb),
        '{MarketingRequest}',
        '"https://n8n.bashpty.com/webhook-test/marketing-request"'
    )::text,
    "UpdatedAt" = NOW()
WHERE "TenantId" = 'eabb1423-ca22-4f96-817d-d068a5c5fd5f';

-- Verificar el cambio
SELECT 
    "TenantId",
    "WebhookUrlsJson"::jsonb->>'MarketingRequest' as "MarketingRequest URL"
FROM "TenantN8nConfigs"
WHERE "TenantId" = 'eabb1423-ca22-4f96-817d-d068a5c5fd5f';


