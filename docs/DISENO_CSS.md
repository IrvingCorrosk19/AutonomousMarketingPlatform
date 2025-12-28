# Diseño CSS Personalizado - Romper el Look AdminLTE

## Objetivo del Diseño

Transformar AdminLTE en un diseño:
- ✅ **Profesional:** Aspecto corporativo y serio
- ✅ **Sobrio:** Colores discretos, no llamativos
- ✅ **Minimalista:** Sin elementos decorativos innecesarios
- ✅ **Enfocado en confianza:** Transmite seguridad y control
- ✅ **Enfocado en automatización:** Destaca el poder del sistema
- ✅ **Estilo "producto serio":** No marketing barato

## Paleta de Colores

### Colores Principales

```css
/* Grises profesionales */
--color-primary: #2C3E50;        /* Azul oscuro profesional */
--color-secondary: #34495E;       /* Gris azulado */
--color-accent: #3498DB;          /* Azul discreto (no brillante) */
--color-success: #27AE60;         /* Verde sobrio */
--color-warning: #F39C12;         /* Ámbar discreto */
--color-danger: #E74C3C;          /* Rojo sobrio */
--color-info: #5DADE2;            /* Azul claro profesional */

/* Grises neutros */
--color-gray-50: #F8F9FA;
--color-gray-100: #E9ECEF;
--color-gray-200: #DEE2E6;
--color-gray-300: #CED4DA;
--color-gray-400: #ADB5BD;
--color-gray-500: #6C757D;
--color-gray-600: #495057;
--color-gray-700: #343A40;
--color-gray-800: #212529;
--color-gray-900: #1A1D20;

/* Fondo y texto */
--bg-primary: #FFFFFF;
--bg-secondary: #F8F9FA;
--bg-dark: #1A1D20;
--text-primary: #212529;
--text-secondary: #6C757D;
--text-muted: #ADB5BD;
```

### Uso de Colores

- **Primary:** Sidebar, botones principales, enlaces activos
- **Secondary:** Fondos alternos, bordes sutiles
- **Accent:** Elementos destacados, estados hover discretos
- **Success/Warning/Danger:** Estados y alertas (colores suavizados)

## Tipografías

### Fuente Principal

```css
font-family: 'Inter', 'Source Sans Pro', -apple-system, BlinkMacSystemFont, 
             'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
```

**Características:**
- **Inter** como fuente principal (moderna, legible, profesional)
- **Source Sans Pro** como fallback (ya incluida en AdminLTE)
- Tamaños de fuente generosos para legibilidad
- Line-height cómodo (1.6 para texto, 1.4 para títulos)

### Escala Tipográfica

```css
--font-size-xs: 0.75rem;    /* 12px */
--font-size-sm: 0.875rem;   /* 14px */
--font-size-base: 1rem;     /* 16px */
--font-size-lg: 1.125rem;   /* 18px */
--font-size-xl: 1.25rem;    /* 20px */
--font-size-2xl: 1.5rem;    /* 24px */
--font-size-3xl: 1.875rem;  /* 30px */
--font-size-4xl: 2.25rem;   /* 36px */
```

## Espaciados

### Sistema de Espaciado

```css
--spacing-xs: 0.25rem;   /* 4px */
--spacing-sm: 0.5rem;    /* 8px */
--spacing-md: 1rem;      /* 16px */
--spacing-lg: 1.5rem;    /* 24px */
--spacing-xl: 2rem;      /* 32px */
--spacing-2xl: 3rem;     /* 48px */
--spacing-3xl: 4rem;     /* 64px */
```

**Aplicación:**
- Padding en cards: `1.5rem` (24px)
- Margen entre secciones: `2rem` (32px)
- Espaciado interno de elementos: `1rem` (16px)

## Cards

### Estilo de Cards

```css
.card {
    border: 1px solid var(--color-gray-200);
    border-radius: 4px;              /* Bordes ligeramente redondeados */
    box-shadow: 0 1px 3px rgba(0,0,0,0.08);  /* Sombra sutil */
    background: var(--bg-primary);
    transition: box-shadow 0.2s ease;
}

.card:hover {
    box-shadow: 0 2px 6px rgba(0,0,0,0.12);  /* Sombra ligeramente más pronunciada */
}

.card-header {
    background: var(--bg-secondary);
    border-bottom: 1px solid var(--color-gray-200);
    padding: 1rem 1.5rem;
    font-weight: 600;
}
```

**Características:**
- Sin bordes gruesos
- Sombras sutiles (no exageradas)
- Bordes redondeados mínimos (4px)
- Hover discreto

## Estados Visuales

### Botones

```css
.btn {
    border-radius: 4px;
    font-weight: 500;
    padding: 0.5rem 1.25rem;
    transition: all 0.2s ease;
    border: none;
}

.btn-primary {
    background: var(--color-primary);
    color: white;
}

.btn-primary:hover {
    background: var(--color-secondary);
    transform: translateY(-1px);
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}
```

### Badges

```css
.badge {
    border-radius: 4px;
    padding: 0.25rem 0.75rem;
    font-weight: 500;
    font-size: 0.75rem;
}
```

### Alertas

```css
.alert {
    border-radius: 4px;
    border-left: 4px solid;
    padding: 1rem 1.5rem;
    background: var(--bg-secondary);
}

.alert-success {
    border-left-color: var(--color-success);
    background: #F0F9F4;
}

.alert-warning {
    border-left-color: var(--color-warning);
    background: #FEF5E7;
}

.alert-danger {
    border-left-color: var(--color-danger);
    background: #FDF2F2;
}
```

## Sidebar

### Customización del Sidebar

```css
.main-sidebar {
    background: var(--color-primary) !important;  /* Sobrescribe AdminLTE */
    box-shadow: 2px 0 8px rgba(0,0,0,0.1);
}

.sidebar .nav-link {
    border-radius: 4px;
    margin: 0.25rem 0.5rem;
    padding: 0.75rem 1rem;
    transition: all 0.2s ease;
}

.sidebar .nav-link:hover {
    background: rgba(255,255,255,0.1);
    transform: translateX(4px);
}

.sidebar .nav-link.active {
    background: var(--color-accent);
    color: white;
    font-weight: 600;
}
```

**Características:**
- Fondo oscuro profesional (no azul brillante de AdminLTE)
- Hover sutil con desplazamiento
- Active state con color accent discreto

## Navbar

### Customización del Navbar

```css
.navbar {
    background: white !important;
    border-bottom: 1px solid var(--color-gray-200);
    box-shadow: 0 1px 3px rgba(0,0,0,0.05);
    padding: 0.75rem 1rem;
}

.navbar .nav-link {
    color: var(--text-primary);
    padding: 0.5rem 1rem;
    border-radius: 4px;
    transition: all 0.2s ease;
}

.navbar .nav-link:hover {
    background: var(--bg-secondary);
}
```

## Cómo Evitar el Look AdminLTE Default

### 1. Sobrescribir Colores

- Reemplazar el azul brillante (#007bff) por grises profesionales
- Usar colores más sobrios y corporativos

### 2. Reducir Sombras

- AdminLTE usa sombras excesivas
- Reducir a sombras sutiles (0 1px 3px)

### 3. Bordes Más Sutiles

- AdminLTE usa bordes gruesos
- Usar bordes finos (1px) y colores discretos

### 4. Espaciado Generoso

- AdminLTE puede sentirse apretado
- Aumentar padding y margins para respiración

### 5. Tipografía Mejorada

- Usar Inter o similar (más moderna que la default)
- Aumentar line-height para legibilidad

### 6. Eliminar Elementos Decorativos

- Quitar efectos hover exagerados
- Eliminar animaciones innecesarias
- Simplificar iconografía

## Responsive Design

```css
/* Mobile */
@media (max-width: 768px) {
    .sidebar {
        transform: translateX(-100%);
    }
    
    .content-wrapper {
        margin-left: 0;
    }
}

/* Tablet */
@media (max-width: 992px) {
    .card {
        margin-bottom: 1rem;
    }
}
```

## Accesibilidad

- Contraste mínimo 4.5:1 para texto normal
- Contraste mínimo 3:1 para texto grande
- Focus states visibles
- Navegación por teclado funcional

