# RiskTrackSCF - API de Autenticación
## Descripción

**RiskTrackLoginApi** es el microservicio central de autenticación para el ecosistema **RiskTrackSCF**. Construido con **.NET**, su única y crucial responsabilidad es verificar las credenciales de los usuarios y, si son correctas, generar y emitir un **JSON Web Token (JWT)**.

Este servicio es el primer punto de contacto para cualquier usuario que intente acceder al sistema a través de una interfaz de usuario como `RiskTrackSCF_ManagmentWEB`.

## Rol en el Ecosistema

El flujo de trabajo es el siguiente:

1.  El usuario introduce su nombre de usuario y contraseña en la aplicación frontend (Blazor).
2.  La aplicación frontend envía estas credenciales a `RiskTrackLoginApi`.
3.  Esta API valida las credenciales contra la base de datos de usuarios.
4.  Si la validación es exitosa, genera un JWT firmado, envia un codigo al correo para factor doble autenticación.
5.  El frontend almacena recibe este codigo y si concuerda con el enviado permite el ingreso al usuario.

## Requisitos Previos

*   **.NET SDK** (se recomienda .NET 6.0 o superior).
*   **Microsoft SQL Server** (debe tener acceso a la base de datos donde se almacenan los usuarios).

## Instalación y Ejecución

1.  **Clona el repositorio:**
    ```bash
    git clone https://github.com/Kpiiale/RiskTrackLoginApi.git
    cd RiskTrackLoginApi
    ```

2.  **Configura la aplicación:**
    *   Busca el archivo `appsettings.json` en el proyecto.
    *   Modifica los valores de configuración.

3.  **Ejecuta la aplicación desde la terminal:**
    ```bash
    dotnet restore
    dotnet run
    ```
    La API de autenticación se iniciará en la URL especificada en `Properties/launchSettings.json`.

## Guía de Configuración (`appsettings.json`)

### Template de Configuración

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR-SERVER-ADDRESS,PORT;Database=YOUR-DATABASE-NAME;User Id=YOUR-DB-USER;Password=YOUR-DB-PASSWORD;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR-VERY-LONG-AND-SECURE-SECRET-KEY-FOR-JWT",
    "Issuer": "RiskTrackAuth",
    "Audience": "RiskTrackUsers",
    "ExpireMinutes": 60
  },
"Smtp": {
    "Host": "smtp.YOUR-EMAIL-SERVICE.com",
    "Port": 587,
    "SenderName": "YOUR-APP-NAME",
    "User": "YOUR-EMAIL-ADDRESS",
    "Password": "YOUR-EMAIL-APP-PASSWORD"
  },

  "RabbitMQ": {
    "HostName": "YOUR-RABBITMQ-HOST",
    "UserName": "YOUR-RABBITMQ-USER",
    "Password": "YOUR-RABBITMQ-PASSWORD"
  }
}
