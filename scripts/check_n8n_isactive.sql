-- Verificar IsActive y la URL del webhook
SELECT 
    "TenantId",
    "IsActive",
    "WebhookUrlsJson"::jsonb->>'MarketingRequest' as "MarketingRequest URL"
FROM "TenantN8nConfigs"
WHERE "TenantId" = 'eabb1423-ca22-4f96-817d-d068a5c5fd5f';


