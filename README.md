# Rotisería LDK - Sistema de Gestión y Punto de Venta (POS)

Sistema integral de gestión de pedidos, clientes y finanzas desarrollado para un negocio gastronómico real. Diseñado bajo una arquitectura de red local (LAN) para operar con cero costos de infraestructura en la nube (On-Premise), permitiendo a múltiples dispositivos (celulares, tablets) conectarse a una PC central.

## Características Principales

- **Punto de Venta (POS):** Toma de pedidos rápida con cálculo automático de totales y costos de envío.
- **Gestión de Deudores (Cuenta Corriente):** Sistema automatizado para registrar "fiados" y cobrar saldos pendientes.
- **Control de Stock Automatizado:** Descuento en tiempo real de unidades cerradas (bebidas) y alertas de bajo stock.
- **Dashboard Financiero:** Métricas en tiempo real separando ingresos operativos (comida) de ingresos logísticos (delivery).
- **Historial y Reportes:** Gráficos mensuales y detalle de ventas diarias.
- **Gestión de Menú (Soft Delete):** Activación/Desactivación de productos sin afectar la integridad referencial de los reportes históricos.

## Tecnologías Utilizadas

- **Backend:** C# / .NET 8 (ASP.NET Core Web API).
- **Frontend:** Blazor WebAssembly / Server, estilizado con MudBlazor (Material Design).
- **Base de Datos:** SQLite (Ideal para despliegues locales sin configuración de servidores de bases de datos) y Entity Framework Core.
- **Arquitectura:** RESTful API, patrón MVC adaptado, inyección de dependencias.

## Instalación y Despliegue Local (Entorno de Producción LAN)

Este sistema está diseñado para ejecutarse en una red local (WiFi del negocio).

1.  **Clonar el repositorio:**
    `git clone https://github.com/tu-usuario/rotiserialdk.git`
2.  **Publicar la API y la Web:**
    `dotnet publish -c Release -o ./Publicacion`
3.  **Configuración de Red (Binding):**
    Asegurarse de que el servidor escuche en `0.0.0.0` en lugar de `localhost` para permitir conexiones entrantes en la LAN.
4.  **Ejecución:**
    Trasladar los archivos publicados a la PC principal del negocio. El resto de los dispositivos accederán mediante la IP local asignada al servidor (Ej: `http://192.168.1.X:5080`).
