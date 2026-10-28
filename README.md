# Nexo Gestion de eventos

Aplicación ASP.NET Core MVC para administrar eventos, asistentes e inscripciones mediante Entity Framework Core Code First y SQL Server.

## Funcionalidades

- Eventos con categoría, modalidad, aforo, precio y eliminación lógica.
- Asistentes con validación de DNI y correo únicos.
- Inscripciones con control de fecha, duplicidad y cupos.
- Cancelación lógica de inscripciones.
- Tres migraciones de Entity Framework Core.
- Interfaz responsiva con la identidad Nexo Eventos.

## Requisitos

- .NET SDK 10
- Docker Desktop
- `dotnet-ef` 10

## Configuración local

Crea `Eventos.Web/appsettings.Development.json` con:

```json
{
  "ConnectionStrings": {
    "Eventos": "Server=localhost,1433;Database=EventosDB;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True;Encrypt=True"
  }
}
```

Inicia SQL Server y la aplicación:

```bash
docker start eventos-sql
cd Eventos.Web
dotnet ef database update
dotnet run
```

## Migraciones

```bash
dotnet ef migrations list
```

El proyecto conserva exactamente estas tres migraciones:

- `InitialCreate`
- `AgregarModalidadEvento`
- `AgregarTelefonoAsistente`
