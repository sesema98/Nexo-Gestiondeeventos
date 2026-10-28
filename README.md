# Nexo Gestion de eventos

Aplicación web desarrollada con ASP.NET Core MVC, Entity Framework Core Code First y SQL Server. Permite administrar eventos, asistentes e inscripciones desde una interfaz web responsiva.

## Funcionalidades

- Registro, consulta, edición y eliminación lógica de eventos.
- Filtro por categoría y control de aforo, inscritos y cupos.
- Modalidades presencial y virtual.
- Asistentes con validación de DNI y correo únicos.
- Inscripciones con validación de fecha, duplicidad y aforo.
- Cancelación lógica de inscripciones.
- Datos iniciales: 3 categorías, 4 eventos y 5 asistentes.
- Tres migraciones de Entity Framework Core.

## Tecnologías

- .NET 10 y ASP.NET Core MVC
- Entity Framework Core 10
- SQL Server
- Bootstrap
- Docker Desktop

## Requisitos previos

Instala [.NET SDK 10](https://dotnet.microsoft.com/download), [Docker Desktop](https://www.docker.com/products/docker-desktop/), Git y Visual Studio Code o Visual Studio.

Comprueba las instalaciones:

```bash
dotnet --version
docker --version
git --version
```

## 1. Clonar el repositorio

```bash
git clone https://github.com/sesema98/Nexo-Gestiondeeventos.git
cd Nexo-Gestiondeeventos/Eventos.Web
```

## 2. Instalar dotnet ef

```bash
dotnet tool install --global dotnet-ef --version 10.0.0
```

Si ya está instalado:

```bash
dotnet tool update --global dotnet-ef --version 10.0.0
```

En macOS o Linux, si no se reconoce la herramienta:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

## 3. Restaurar y compilar

Desde `Eventos.Web`:

```bash
dotnet restore
dotnet build
```

## 4. Iniciar SQL Server

La contraseña debe incluir mayúsculas, minúsculas, números y símbolos. El ejemplo usa una contraseña solo para desarrollo local.

### Windows o equipo Intel AMD64

```bash
docker run -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD='NexoEventos2026!' -p 1433:1433 --name eventos-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

En PowerShell:

```powershell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=NexoEventos2026!" -p 1433:1433 --name eventos-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

### Mac con Apple Silicon

```bash
docker run -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD='NexoEventos2026!' -p 1433:1433 --name eventos-sql -d mcr.microsoft.com/azure-sql-edge:latest
```

Comprueba el contenedor:

```bash
docker ps
docker logs eventos-sql
```

En ejecuciones posteriores:

```bash
docker start eventos-sql
```

## 5. Configurar la conexión

Crea `Eventos.Web/appsettings.Development.json`. Este archivo está excluido de Git para no publicar contraseñas.

```json
{
  "ConnectionStrings": {
    "Eventos": "Server=localhost,1433;Database=EventosDB;User Id=sa;Password=NexoEventos2026!;TrustServerCertificate=True;Encrypt=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

La contraseña debe coincidir con la utilizada al crear el contenedor.

## 6. Crear EventosDB

Las tres migraciones ya están incluidas. No vuelvas a ejecutar `dotnet ef migrations add`.

```bash
dotnet ef database update
```

Esto crea `EventosDB`, las tablas y `__EFMigrationsHistory`. Comprueba el resultado:

```bash
dotnet ef migrations list
```

Debe mostrar:

```text
InitialCreate
AgregarModalidadEvento
AgregarTelefonoAsistente
```

## 7. Ejecutar la aplicación

```bash
dotnet run --urls http://localhost:5088
```

Abre:

```text
http://localhost:5088/Eventos
```

La primera ejecución carga los datos iniciales si las tablas están vacías.

## Migraciones incluidas

1. `InitialCreate`: crea las cuatro entidades y sus relaciones.
2. `AgregarModalidadEvento`: agrega `Modalidad` con `Presencial` como valor inicial.
3. `AgregarTelefonoAsistente`: agrega el teléfono opcional de máximo 9 caracteres.

Los comandos utilizados originalmente fueron:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update

dotnet ef migrations add AgregarModalidadEvento
# Antes de aplicarla se cambió defaultValue a "Presencial".
dotnet ef database update

dotnet ef migrations add AgregarTelefonoAsistente
dotnet ef database update
```

En un clon del repositorio solo necesitas ejecutar `dotnet ef database update`.

## Verificar el historial

Conéctate desde la extensión SQL Server de VS Code o SQL Server Management Studio:

```text
Servidor: localhost,1433
Base: EventosDB
Usuario: sa
Contraseña: la configurada localmente
Trust server certificate: activado
```

Ejecuta:

```sql
USE EventosDB;

SELECT MigrationId, ProductVersion
FROM __EFMigrationsHistory
ORDER BY MigrationId;
```

## Comandos útiles

```bash
dotnet build
dotnet ef migrations list
docker stop eventos-sql
docker start eventos-sql
```

## Problemas frecuentes

### SQL Server rechaza la conexión

```bash
docker ps
docker logs eventos-sql
```

Comprueba el puerto `1433` y que la contraseña coincida con `appsettings.Development.json`.

### El nombre eventos-sql ya existe

```bash
docker start eventos-sql
```

### EventosDB todavía no existe

```bash
dotnet ef database update
```

### Error de certificado

La conexión debe incluir:

```text
TrustServerCertificate=True;Encrypt=True
```

## Seguridad

- No publiques `appsettings.Development.json`.
- No uses la contraseña de ejemplo en producción.
- Usa variables de entorno o un administrador de secretos en ambientes reales.
