-- Script para verificar la tabla ApplicationLogs

-- 1. Verificar que la tabla existe
SELECT table_name, table_type 
FROM information_schema.tables 
WHERE table_name = 'ApplicationLogs';

-- 2. Ver todas las columnas de la tabla
SELECT 
    column_name, 
    data_type, 
    character_maximum_length, 
    is_nullable, 
    column_default
FROM information_schema.columns 
WHERE table_name = 'ApplicationLogs' 
ORDER BY ordinal_position;

-- 3. Ver todos los índices
SELECT 
    indexname, 
    indexdef 
FROM pg_indexes 
WHERE tablename = 'ApplicationLogs' 
ORDER BY indexname;

-- 4. Contar registros (debería ser 0 inicialmente)
SELECT COUNT(*) as total_logs 
FROM "ApplicationLogs";

-- 5. Ver comentarios de la tabla
SELECT 
    obj_description('public."ApplicationLogs"'::regclass) as table_comment;

-- 6. Ver comentarios de las columnas
SELECT 
    column_name,
    col_description('public."ApplicationLogs"'::regclass, ordinal_position) as column_comment
FROM information_schema.columns 
WHERE table_name = 'ApplicationLogs'
ORDER BY ordinal_position;

