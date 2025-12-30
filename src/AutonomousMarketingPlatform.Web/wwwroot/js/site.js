// Site-wide JavaScript for Autonomous Marketing Platform

$(document).ready(function() {
    // Inicialización general
    console.log('Autonomous Marketing Platform loaded');
    
    // AdminLTE se inicializa automáticamente al cargar adminlte.min.js
    // Pero podemos forzar la inicialización de widgets si es necesario
    
    // Pushmenu (botón de menú hamburguesa) - AdminLTE lo maneja automáticamente
    // Si no funciona, inicializamos manualmente
    $('[data-widget="pushmenu"]').on('click', function(e) {
        e.preventDefault();
        if ($('body').hasClass('sidebar-collapse')) {
            $('body').removeClass('sidebar-collapse');
        } else {
            $('body').addClass('sidebar-collapse');
        }
    });
    
    // Treeview (menú lateral) - AdminLTE lo maneja automáticamente con data-widget="treeview"
    
    // Card widget refresh
    $('[data-card-widget="refresh"]').on('click', function(e) {
        e.preventDefault();
        var $card = $(this).closest('.card');
        var source = $(this).data('source');
        
        // Agregar clase de loading
        $card.addClass('card-refreshing');
        
        if (source) {
            // Hacer petición AJAX para refrescar
            $.get(source)
                .done(function(data) {
                    console.log('Card refreshed', data);
                    // Aquí podrías actualizar el contenido de la card con los nuevos datos
                })
                .fail(function() {
                    console.error('Error refreshing card');
                })
                .always(function() {
                    setTimeout(function() {
                        $card.removeClass('card-refreshing');
                    }, 500);
                });
        } else {
            // Si no hay source, solo simular refresh
            setTimeout(function() {
                $card.removeClass('card-refreshing');
            }, 500);
        }
    });
    
    // Tooltips de Bootstrap
    $('[data-toggle="tooltip"]').tooltip();
    
    // Popovers de Bootstrap
    $('[data-toggle="popover"]').popover();
    
    // Dropdowns de Bootstrap (asegurar que funcionen)
    $('.dropdown-toggle').dropdown();
    
    // Inicializar DataTables si existen tablas
    if ($.fn.DataTable) {
        $('.data-table').DataTable({
            "language": {
                "url": "//cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json"
            },
            "responsive": true,
            "pageLength": 25
        });
    }
    
    console.log('AdminLTE widgets initialized');
});

