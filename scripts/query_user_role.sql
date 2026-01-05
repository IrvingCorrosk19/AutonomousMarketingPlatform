-- Query para verificar el rol del usuario admin@test.com
-- Este query muestra el usuario, su tenant, y todos sus roles asignados

SELECT 
    u."Id" AS "UserId",
    u."Email",
    u."UserName",
    u."FullName",
    u."TenantId",
    t."Name" AS "TenantName",
    r."Name" AS "RoleName",
    r."Id" AS "RoleId",
    ut."IsPrimary" AS "IsPrimaryTenant",
    ut."JoinedAt" AS "JoinedAt"
FROM "AspNetUsers" u
LEFT JOIN "Tenants" t ON u."TenantId" = t."Id"
LEFT JOIN "UserTenants" ut ON u."Id" = ut."UserId"
LEFT JOIN "AspNetRoles" r ON ut."RoleId" = r."Id"
WHERE u."Email" = 'admin@test.com'
ORDER BY ut."IsPrimary" DESC, ut."JoinedAt" DESC;

-- También puedes verificar los roles directamente de Identity:
SELECT 
    u."Email",
    r."Name" AS "RoleName"
FROM "AspNetUsers" u
INNER JOIN "AspNetUserRoles" ur ON u."Id" = ur."UserId"
INNER JOIN "AspNetRoles" r ON ur."RoleId" = r."Id"
WHERE u."Email" = 'admin@test.com';

