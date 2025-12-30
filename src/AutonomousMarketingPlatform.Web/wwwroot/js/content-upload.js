// Gestión de carga de archivos múltiples con vista previa

(function() {
    'use strict';

    const fileInput = document.getElementById('fileInput');
    const uploadForm = document.getElementById('uploadForm');
    const uploadBtn = document.getElementById('uploadBtn');
    const clearBtn = document.getElementById('clearBtn');
    const previewCard = document.getElementById('previewCard');
    const previewContainer = document.getElementById('previewContainer');
    const progressCard = document.getElementById('progressCard');
    const progressBar = document.getElementById('progressBar');
    const uploadStatus = document.getElementById('uploadStatus');
    const resultsCard = document.getElementById('resultsCard');
    const resultsContainer = document.getElementById('resultsContainer');

    let selectedFiles = [];

    // Actualizar label del input file
    fileInput.addEventListener('change', function(e) {
        const files = Array.from(e.target.files);
        if (files.length > 0) {
            const label = fileInput.nextElementSibling;
            label.textContent = `${files.length} archivo(s) seleccionado(s)`;
            handleFileSelection(files);
        }
    });

    // Manejar selección de archivos
    function handleFileSelection(files) {
        selectedFiles = files;
        uploadBtn.disabled = false;
        clearBtn.style.display = 'inline-block';
        
        // Validar archivos
        const validationResults = validateFiles(files);
        
        if (validationResults.errors.length > 0) {
            showValidationErrors(validationResults.errors);
            uploadBtn.disabled = true;
            return;
        }

        // Mostrar vista previa
        showPreview(files);
    }

    // Validar archivos
    function validateFiles(files) {
        const errors = [];
        const maxSize = 100 * 1024 * 1024; // 100 MB
        const allowedTypes = [
            'image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp',
            'video/mp4', 'video/mpeg', 'video/quicktime', 'video/x-msvideo', 'video/webm'
        ];
        const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp', '.mp4', '.mpeg', '.mov', '.avi', '.webm'];

        files.forEach(file => {
            // Validar tamaño
            if (file.size === 0) {
                errors.push(`${file.name}: El archivo está vacío.`);
            }
            if (file.size > maxSize) {
                errors.push(`${file.name}: Excede el tamaño máximo de 100 MB.`);
            }

            // Validar tipo MIME
            if (!allowedTypes.includes(file.type.toLowerCase())) {
                errors.push(`${file.name}: Tipo de archivo no permitido.`);
            }

            // Validar extensión
            const extension = '.' + file.name.split('.').pop().toLowerCase();
            if (!allowedExtensions.includes(extension)) {
                errors.push(`${file.name}: Extensión no permitida.`);
            }
        });

        return {
            isValid: errors.length === 0,
            errors: errors
        };
    }

    // Mostrar errores de validación
    function showValidationErrors(errors) {
        const errorHtml = '<div class="alert alert-danger"><ul class="mb-0">' +
            errors.map(e => `<li>${e}</li>`).join('') +
            '</ul></div>';
        previewContainer.innerHTML = errorHtml;
        previewCard.style.display = 'block';
    }

    // Mostrar vista previa
    function showPreview(files) {
        previewContainer.innerHTML = '';
        
        // Limpiar URLs de objetos anteriores para evitar memory leaks
        if (window.previewObjectURLs) {
            window.previewObjectURLs.forEach(url => URL.revokeObjectURL(url));
        }
        window.previewObjectURLs = [];
        
        files.forEach((file, index) => {
            const col = document.createElement('div');
            col.className = 'col-md-3 col-sm-4 col-6 mb-3';
            
            const preview = document.createElement('div');
            preview.className = 'file-preview';
            preview.dataset.index = index;
            preview.dataset.fileName = file.name;

            if (file.type.startsWith('image/')) {
                const img = document.createElement('img');
                const objectURL = URL.createObjectURL(file);
                window.previewObjectURLs.push(objectURL);
                img.src = objectURL;
                img.alt = file.name;
                img.style.width = '100%';
                img.style.height = '200px';
                img.style.objectFit = 'cover';
                img.style.cursor = 'pointer';
                img.onclick = () => {
                    // Mostrar imagen en modal o pantalla completa
                    showImageModal(objectURL, file.name);
                };
                preview.appendChild(img);
            } else if (file.type.startsWith('video/')) {
                const video = document.createElement('video');
                const objectURL = URL.createObjectURL(file);
                window.previewObjectURLs.push(objectURL);
                video.src = objectURL;
                video.controls = true;
                video.style.width = '100%';
                video.style.maxHeight = '200px';
                video.style.objectFit = 'cover';
                preview.appendChild(video);
            } else {
                // Para otros tipos de archivo, mostrar ícono
                const icon = document.createElement('div');
                icon.className = 'text-center p-3';
                icon.style.border = '2px dashed #ddd';
                icon.style.borderRadius = '4px';
                icon.innerHTML = `
                    <i class="fas fa-file fa-3x text-muted mb-2"></i>
                    <p class="mb-0 text-muted"><small>${file.name}</small></p>
                `;
                preview.appendChild(icon);
            }

            // Información del archivo
            const info = document.createElement('div');
            info.className = 'file-info mt-2';
            info.innerHTML = `
                <strong title="${file.name}">${truncateFileName(file.name, 20)}</strong><br>
                <small class="text-muted">${formatFileSize(file.size)}</small>
            `;
            preview.appendChild(info);

            // Botón para eliminar
            const removeBtn = document.createElement('button');
            removeBtn.className = 'remove-file';
            removeBtn.type = 'button';
            removeBtn.innerHTML = '<i class="fas fa-times"></i>';
            removeBtn.title = 'Eliminar archivo';
            removeBtn.onclick = (e) => {
                e.stopPropagation();
                removeFile(index);
            };
            preview.appendChild(removeBtn);

            col.appendChild(preview);
            previewContainer.appendChild(col);
        });

        previewCard.style.display = 'block';
    }

    // Truncar nombre de archivo si es muy largo
    function truncateFileName(fileName, maxLength) {
        if (fileName.length <= maxLength) return fileName;
        const extension = fileName.split('.').pop();
        const nameWithoutExt = fileName.substring(0, fileName.lastIndexOf('.'));
        const truncatedName = nameWithoutExt.substring(0, maxLength - extension.length - 3);
        return truncatedName + '...' + extension;
    }

    // Mostrar imagen en modal
    function showImageModal(imageSrc, imageName) {
        // Crear modal si no existe
        let modal = document.getElementById('imagePreviewModal');
        if (!modal) {
            modal = document.createElement('div');
            modal.id = 'imagePreviewModal';
            modal.className = 'modal fade';
            modal.innerHTML = `
                <div class="modal-dialog modal-lg modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Vista Previa: <span id="modalImageTitle"></span></h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body text-center">
                            <img id="modalImagePreview" src="" alt="" class="img-fluid" style="max-height: 70vh;">
                        </div>
                    </div>
                </div>
            `;
            document.body.appendChild(modal);
        }
        
        const modalImg = document.getElementById('modalImagePreview');
        const modalTitle = document.getElementById('modalImageTitle');
        modalImg.src = imageSrc;
        modalImg.alt = imageName;
        if (modalTitle) {
            modalTitle.textContent = imageName;
        }
        
        // Mostrar modal usando Bootstrap
        $(modal).modal('show');
    }

    // Eliminar archivo de la selección
    function removeFile(index) {
        // Revocar URL del objeto antes de eliminar
        const fileToRemove = selectedFiles[index];
        const previewElement = previewContainer.querySelector(`[data-index="${index}"]`);
        if (previewElement) {
            const img = previewElement.querySelector('img');
            if (img && img.src.startsWith('blob:')) {
                URL.revokeObjectURL(img.src);
            }
            const video = previewElement.querySelector('video');
            if (video && video.src.startsWith('blob:')) {
                URL.revokeObjectURL(video.src);
            }
        }
        
        selectedFiles.splice(index, 1);
        
        // Actualizar input file
        const dataTransfer = new DataTransfer();
        selectedFiles.forEach(file => dataTransfer.items.add(file));
        fileInput.files = dataTransfer.files;
        
        // Actualizar label
        const label = fileInput.nextElementSibling;
        if (selectedFiles.length === 0) {
            label.textContent = 'Elegir archivos...';
        } else {
            label.textContent = `${selectedFiles.length} archivo(s) seleccionado(s)`;
        }
        
        // Actualizar vista previa
        if (selectedFiles.length === 0) {
            previewCard.style.display = 'none';
            uploadBtn.disabled = true;
            clearBtn.style.display = 'none';
        } else {
            showPreview(selectedFiles);
        }
    }

    // Limpiar selección
    clearBtn.addEventListener('click', function() {
        // Revocar todas las URLs de objetos
        if (window.previewObjectURLs) {
            window.previewObjectURLs.forEach(url => URL.revokeObjectURL(url));
            window.previewObjectURLs = [];
        }
        
        selectedFiles = [];
        fileInput.value = '';
        fileInput.nextElementSibling.textContent = 'Elegir archivos...';
        previewCard.style.display = 'none';
        uploadBtn.disabled = true;
        clearBtn.style.display = 'none';
        resultsCard.style.display = 'none';
        previewContainer.innerHTML = '';
    });

    // Enviar formulario
    uploadForm.addEventListener('submit', async function(e) {
        e.preventDefault();

        if (selectedFiles.length === 0) {
            alert('Por favor, seleccione al menos un archivo.');
            return;
        }

        // Ocultar resultados anteriores
        resultsCard.style.display = 'none';

        // Mostrar progreso
        progressCard.style.display = 'block';
        progressBar.style.width = '0%';
        progressBar.textContent = '0%';
        uploadStatus.innerHTML = 'Preparando archivos...';
        uploadBtn.disabled = true;

        try {
            const formData = new FormData();
            selectedFiles.forEach(file => {
                formData.append('files', file);
            });

            const description = document.getElementById('description').value;
            const tags = document.getElementById('tags').value;
            const campaignId = document.getElementById('campaignId').value;

            if (description) formData.append('description', description);
            if (tags) formData.append('tags', tags);
            if (campaignId) formData.append('campaignId', campaignId);

            // Simular progreso
            let progress = 0;
            const progressInterval = setInterval(() => {
                progress += 10;
                if (progress <= 90) {
                    progressBar.style.width = progress + '%';
                    progressBar.textContent = progress + '%';
                }
            }, 200);

            const response = await fetch('/Content/UploadFiles', {
                method: 'POST',
                body: formData
            });

            clearInterval(progressInterval);
            progressBar.style.width = '100%';
            progressBar.textContent = '100%';

            if (!response.ok) {
                throw new Error('Error al cargar los archivos.');
            }

            const result = await response.json();
            showResults(result);

            // Limpiar formulario después de éxito
            setTimeout(() => {
                clearBtn.click();
            }, 3000);

        } catch (error) {
            console.error('Error:', error);
            uploadStatus.innerHTML = `<div class="alert alert-danger">Error: ${error.message}</div>`;
        } finally {
            uploadBtn.disabled = false;
            setTimeout(() => {
                progressCard.style.display = 'none';
            }, 2000);
        }
    });

    // Mostrar resultados
    function showResults(result) {
        resultsContainer.innerHTML = '';

        if (result.successfulUploads > 0) {
            const successHtml = `
                <div class="alert alert-success">
                    <h5><i class="fas fa-check-circle"></i> Carga Exitosa</h5>
                    <p>Se cargaron correctamente ${result.successfulUploads} de ${result.totalFiles} archivo(s).</p>
                </div>
            `;
            resultsContainer.innerHTML += successHtml;
        }

        if (result.failedUploads > 0) {
            const errorsHtml = `
                <div class="alert alert-warning">
                    <h5><i class="fas fa-exclamation-triangle"></i> Errores en la Carga</h5>
                    <ul>
                        ${result.errors.map(e => `<li><strong>${e.fileName}:</strong> ${e.errorMessage}</li>`).join('')}
                    </ul>
                </div>
            `;
            resultsContainer.innerHTML += errorsHtml;
        }

        resultsCard.style.display = 'block';
    }

    // Formatear tamaño de archivo
    function formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
    }
})();

