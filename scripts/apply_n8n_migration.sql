-- Migración: AddTenantN8nConfig
-- Aplicar esta migración directamente en PostgreSQL

-- Crear tabla TenantN8nConfigs
CREATE TABLE IF NOT EXISTS "TenantN8nConfigs" (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "UseMock" boolean NOT NULL,
    "BaseUrl" character varying(500) NOT NULL,
    "ApiUrl" character varying(500) NOT NULL,
    "EncryptedApiKey" text NULL,
    "DefaultWebhookUrl" character varying(500) NOT NULL,
    "WebhookUrlsJson" text NOT NULL,
    "LastUsedAt" timestamp with time zone NULL,
    "UsageCount" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_TenantN8nConfigs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TenantN8nConfigs_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE RESTRICT
);

-- Crear índice único en TenantId
CREATE UNIQUE INDEX IF NOT EXISTS "IX_TenantN8nConfigs_TenantId" ON "TenantN8nConfigs" ("TenantId");

-- Verificar que la tabla se creó correctamente
SELECT COUNT(*) as "TableExists" FROM information_schema.tables WHERE table_name = 'TenantN8nConfigs';

