-- Actualizar configuración de n8n con la URL proporcionada
-- Primero obtener el TenantId del tenant de prueba
DO $$
DECLARE
    v_tenant_id UUID;
    v_config_id UUID;
    v_webhook_urls JSONB;
BEGIN
    -- Obtener el primer tenant (o el tenant de prueba)
    SELECT "Id" INTO v_tenant_id FROM "Tenants" LIMIT 1;
    
    IF v_tenant_id IS NULL THEN
        RAISE EXCEPTION 'No se encontró ningún tenant';
    END IF;
    
    -- Verificar si ya existe configuración
    SELECT "Id" INTO v_config_id FROM "TenantN8nConfigs" WHERE "TenantId" = v_tenant_id;
    
    -- Preparar JSON de webhooks
    v_webhook_urls := jsonb_build_object(
        'MarketingRequest', 'https://n8n.bashpty.com/webhook-test/marketing-request'
    );
    
    IF v_config_id IS NULL THEN
        -- Crear nueva configuración
        INSERT INTO "TenantN8nConfigs" (
            "Id", "TenantId", "UseMock", "BaseUrl", "ApiUrl", 
            "DefaultWebhookUrl", "WebhookUrlsJson", "CreatedAt", "IsActive", "UsageCount"
        ) VALUES (
            gen_random_uuid(),
            v_tenant_id,
            false, -- No usar mock, usar conexión real
            'https://n8n.bashpty.com',
            'https://n8n.bashpty.com/api/v1',
            'https://n8n.bashpty.com/webhook-test',
            v_webhook_urls::text,
            NOW(),
            true,
            0
        );
        RAISE NOTICE 'Configuración creada para tenant: %', v_tenant_id;
    ELSE
        -- Actualizar configuración existente
        UPDATE "TenantN8nConfigs"
        SET 
            "UseMock" = false,
            "BaseUrl" = 'https://n8n.bashpty.com',
            "ApiUrl" = 'https://n8n.bashpty.com/api/v1',
            "DefaultWebhookUrl" = 'https://n8n.bashpty.com/webhook-test',
            "WebhookUrlsJson" = v_webhook_urls::text,
            "UpdatedAt" = NOW()
        WHERE "Id" = v_config_id;
        RAISE NOTICE 'Configuración actualizada para tenant: %', v_tenant_id;
    END IF;
END $$;

-- Verificar la configuración
SELECT 
    "Id",
    "TenantId",
    "UseMock",
    "BaseUrl",
    "ApiUrl",
    "DefaultWebhookUrl",
    "WebhookUrlsJson"
FROM "TenantN8nConfigs";

