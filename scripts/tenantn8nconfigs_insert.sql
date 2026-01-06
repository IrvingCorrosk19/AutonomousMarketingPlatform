-- INSERT statement para TenantN8nConfigs
-- Listo para ejecutar en Render

INSERT INTO public."TenantN8nConfigs" ("Id", "TenantId", "UseMock", "BaseUrl", "ApiUrl", "EncryptedApiKey", "DefaultWebhookUrl", "WebhookUrlsJson", "LastUsedAt", "UsageCount", "IsActive", "CreatedAt", "UpdatedAt") 
VALUES (
    'daba0ea6-143a-4e4e-93d6-087172200af1',
    'eabb1423-ca22-4f96-817d-d068a5c5fd5f',
    false,
    'https://n8n.bashpty.com',
    'https://n8n.bashpty.com/api/v1',
    NULL,
    'https://n8n.bashpty.com/webhook',
    '{"MarketingRequest": "https://n8n.bashpty.com/webhook-test/marketing-request"}',
    NULL,
    0,
    true,
    '2025-12-30 07:51:29.928033-08',
    '2025-12-30 08:33:44.334555-08'
);

