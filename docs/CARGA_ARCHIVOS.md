# Módulo de Carga de Imágenes y Videos

## Visión General

Sistema completo para cargar imágenes y videos desde PC o celular, con selección múltiple, vista previa, validaciones y almacenamiento temporal.

## Características Implementadas

### 1. Frontend (Vista y JavaScript)

#### Funcionalidades
- ✅ **Selección múltiple**: Permite seleccionar varios archivos a la vez
- ✅ **Vista previa**: Muestra preview de imágenes y videos antes de cargar
- ✅ **Validaciones en cliente**: Valida tamaño, tipo y extensión antes de enviar
- ✅ **Interfaz intuitiva**: Diseño limpio con AdminLTE personalizado
- ✅ **Progreso de carga**: Barra de progreso durante la carga
- ✅ **Manejo de errores**: Muestra errores específicos por archivo
- ✅ **Eliminación de archivos**: Permite quitar archivos de la selección antes de cargar

#### Validaciones Frontend
- Tamaño máximo: 100 MB por archivo
- Tipos permitidos:
  - **Imágenes**: JPEG, JPG, PNG, GIF, WEBP
  - **Videos**: MP4, MPEG, MOV, AVI, WEBM
- Validación de extensión
- Validación de tipo MIME

### 2. Backend (Application Layer)

#### UploadFilesCommand
- Procesa múltiples archivos simultáneamente
- Valida cada archivo individualmente
- Guarda archivos temporalmente
- Crea registros en base de datos
- Retorna resultado detallado (éxitos y errores)

#### Validaciones Backend
- Tamaño de archivo (0 bytes y máximo 100 MB)
- Tipo MIME permitido
- Extensión de archivo
- Verificación de usuario y tenant

### 3. Servicio de Almacenamiento

#### IFileStorageService / FileStorageService
- **SaveTemporaryFileAsync**: Guarda archivos en carpeta temporal
- **GetPreviewUrl**: Genera URL para vista previa
- **MoveToPermanentAsync**: Mueve archivos de temporal a permanente
- **DeleteTemporaryFileAsync**: Elimina archivos temporales
- **DeletePermanentFileAsync**: Elimina archivos permanentes

#### Estructura de Almacenamiento
```
wwwroot/
  uploads/
    temp/          # Archivos temporales (hasta procesamiento por IA)
    permanent/     # Archivos procesados y aprobados
```

### 4. Controlador

#### ContentController
- **GET /Content/Upload**: Vista de carga de archivos
- **POST /Content/UploadFiles**: Endpoint para recibir archivos
- **GET /Content/Index**: Vista para listar contenido (pendiente)

#### Características
- Límite de tamaño: 100 MB por request
- Soporte para múltiples archivos
- Campos opcionales: descripción, tags, campaña

## Flujo de Trabajo

### 1. Selección de Archivos
1. Usuario hace clic en "Elegir archivos..."
2. Selecciona uno o múltiples archivos
3. JavaScript valida archivos en cliente
4. Si hay errores, se muestran inmediatamente
5. Si son válidos, se muestra vista previa

### 2. Vista Previa
1. Se generan previews de imágenes (thumbnails)
2. Se muestran controles de video para preview
3. Se muestra información de cada archivo (nombre, tamaño)
4. Usuario puede eliminar archivos de la selección

### 3. Carga al Backend
1. Usuario hace clic en "Cargar Archivos"
2. Se muestra barra de progreso
3. Archivos se envían vía FormData
4. Backend valida cada archivo
5. Archivos válidos se guardan temporalmente
6. Se crean registros en base de datos (tabla Contents)
7. Se retorna resultado con éxitos y errores

### 4. Almacenamiento Temporal
- Archivos se guardan en `wwwroot/uploads/temp/`
- Nombre único: `{Guid}_{NombreOriginal}`
- URL de preview: `/uploads/temp/{FileName}`
- Los archivos permanecen temporales hasta procesamiento por IA

## Validaciones

### Frontend (JavaScript)
```javascript
- Tamaño: 0 bytes < tamaño <= 100 MB
- Tipo MIME: image/* o video/*
- Extensión: .jpg, .jpeg, .png, .gif, .webp, .mp4, .mpeg, .mov, .avi, .webm
```

### Backend (C#)
```csharp
- Tamaño: Validación estricta
- Tipo MIME: Lista blanca de tipos permitidos
- Extensión: Validación de extensión
- Usuario/Tenant: Verificación de pertenencia
```

## Estructura de Datos

### FileUploadDto
- Información del archivo cargado
- URL de preview
- Metadata (tamaño, tipo, fecha)

### MultipleFileUploadResponseDto
- Lista de archivos cargados exitosamente
- Lista de errores por archivo
- Estadísticas (total, exitosos, fallidos)

## Integración con IA

**IMPORTANTE**: El frontend solo carga los archivos. El procesamiento por IA se realiza después:

1. Archivos se guardan temporalmente
2. Se crean registros en `Contents` con `IsAiGenerated = false`
3. Un proceso background (futuro) procesará los archivos con IA
4. Después del procesamiento, los archivos se moverán a permanente

## Próximos Pasos

1. ✅ Carga de archivos implementada
2. ✅ Validaciones implementadas
3. ✅ Vista previa implementada
4. ⏳ Integrar con sistema de autenticación (obtener UserId real)
5. ⏳ Proceso background para procesamiento con IA
6. ⏳ Vista para listar contenido cargado
7. ⏳ Funcionalidad para eliminar archivos
8. ⏳ Compresión de imágenes grandes

## Archivos Creados

### Application Layer
- `DTOs/FileUploadDto.cs`
- `UseCases/Content/UploadFilesCommand.cs`
- `Services/IFileStorageService.cs`

### Infrastructure Layer
- `Services/FileStorageService.cs`

### Web Layer
- `Controllers/ContentController.cs`
- `Views/Content/Upload.cshtml`
- `wwwroot/js/content-upload.js`

## Uso

1. Acceder a `/Content/Upload`
2. Seleccionar archivos (múltiple)
3. Ver preview
4. Agregar descripción/tags (opcional)
5. Hacer clic en "Cargar Archivos"
6. Ver resultados

El sistema está listo para recibir archivos y almacenarlos temporalmente hasta su procesamiento por IA.

