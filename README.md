# LDK Rotiseria - POS & Business Management System

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4?style=flat&logo=blazor)
![SQLite](https://img.shields.io/badge/SQLite-Database-003B57?style=flat&logo=sqlite)
![Status](https://img.shields.io/badge/Status-Completed-success?style=flat)

# LDK Rotiseria - POS & Business Management System

A comprehensive Point of Sale (POS) and business management system developed for a real-world gastronomic enterprise. Designed with a local network (LAN) architecture to operate with zero cloud infrastructure costs (On-Premise), allowing multiple devices (smartphones, tablets) to connect to a central PC acting as a local server.

## Key Features

- **Point of Sale (POS):** Agile order processing with automatic calculation of totals and delivery fees.
- **Accounts Receivable (Customer Credit):** Automated tracking of customer debts, pending balances, and payment settlements.
- **Automated Inventory Control:** Real-time stock deduction for sealed units (everages) and low-stock threshold alerts.
- **Financial Dashboard:** Real-time metrics separating operational revenue (food) from logistics revenue (delivery).
- **History & Reporting:** Visual monthly performance charts and detailed daily sales breakdowns.
- **Menu Management (Soft Deletes):** Product activation/deactivation toggles that preserve referential integrity for historical sales reports.

## Tech Stack

- **Backend:** C# / .NET 8 (ASP.NET Core Web API)
- **Frontend:** Blazor WebAssembly / Server, styled with MudBlazor (Material Design)
- **Database:** SQLite (Optimized for local, zero-config deployments) & Entity Framework Core
- **Architecture:** RESTful API, clean architecture principles, Dependency Injection.

## Local Deployment (LAN Production Environment)

This system is specifically engineered to run on a local network (e.g., the store's WiFi) without requiring paid cloud hosting.

1. **Clone the repository:**

   ```bash
   git clone [https://github.com/calebJT7/rotiserialdk.git](https://github.com/calebJT7/rotiserialdk.git)

   ```

2. **Publish the API and Web Client:**
   Compile the optimized release build:

dotnet publish -c Release -o ./Publish

3. **Network Configuration (Binding):**
   Ensure the server is configured to listen on 0.0.0.0 rather than localhost to allow incoming connections from other devices on the LAN.

4. **Execution:**
   Move the published files to the business's main PC. Mobile devices and tablets can access the system via the server's local IP address (e.g., http://192.168.1.X:5080).

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

    [![LinkedIn](https://img.shields.io/badge/LinkedIn-Connect_with_me-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/caleb-toledo-356b56336/)
