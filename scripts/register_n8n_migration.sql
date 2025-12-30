-- Registrar la migración en el historial de EF
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250101000000_AddTenantN8nConfig', '8.0.2')
ON CONFLICT DO NOTHING;

-- Verificar que se registró
SELECT * FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250101000000_AddTenantN8nConfig';

