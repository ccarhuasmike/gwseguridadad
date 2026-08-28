# Security — Gestión de Perfiles, Usuarios, Opciones y Acciones

Backend en **.NET 8 / ASP.NET Core Web API** para la administración de
perfiles, usuarios, opciones (menú recursivo) y acciones, con configuración de
permisos Perfil→Opción→Acción y Usuario→Opción→Acción. Implementado con
**CQRS (MediatR)**, **Dapper** y **SQL Server**, siguiendo arquitectura limpia
y principios SOLID.

## 1. Arquitectura

```
src/
├── Security.Domain          Entidades, enumeraciones, excepciones de dominio
├── Security.Application      CQRS: Commands/Queries/Handlers/DTOs/Validadores/Interfaces
├── Security.Infrastructure   Repositorios Dapper, conexión SQL Server, Unit of Work
└── Security.Api               Controllers, middleware global de errores, Swagger, DI
tests/
└── Security.Tests            Pruebas unitarias (xUnit + Moq)
sql/
├── 01_schema_seguridad.sql   DDL del esquema seg.*
└── 02_seed_data.sql          Datos de ejemplo (árbol Sistema/Seguridad/Reportes, etc.)
```

Organización de `Security.Application` por *feature* (una carpeta por
funcionalidad, cada una con sus propios `Commands` y `Queries`):

```
Features/
├── Perfil            (Create/Update/Delete, GetById/GetAll)
├── Opcion            (Create/Update/Delete, GetById/GetAll/GetTree/GetChildren)
├── Accion            (Create/Update/Delete, GetByOpcion/GetAll)
├── PerfilOpcion      (Assign/Remove Opciones y Acciones, SavePerfilPermisos, GetPerfilPermisos)
└── UsuarioOpcion     (Assign/Remove Opciones y Acciones, SaveUsuarioPermisos, GetUsuarioPermisos)
```

## 2. Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local, contenedor Docker, o instancia remota)
- (Opcional) Docker + Docker Compose

## 3. Configuración de la cadena de conexión

La cadena de conexión **nunca está hardcodeada**. Se lee desde
`appsettings.json` / `appsettings.{Environment}.json` o desde variables de
entorno, bajo la clave `ConnectionStrings:SecurityDatabase`:

```json
{
  "ConnectionStrings": {
    "SecurityDatabase": "Server=localhost,1433;Database=DEMO;User Id=sa;TrustServerCertificate=True;"
  }
}
```

Para evitar que la credencial de SQL Server quede embebida como texto plano
en un archivo versionado, la contraseña se puede suministrar por separado
mediante la clave `Db:SqlAuthSecret` (variable de entorno
`Db__SqlAuthSecret`). Si se define, `SqlServerConnectionFactory` la combina en
tiempo de ejecución con la cadena de conexión base usando
`SqlConnectionStringBuilder`. También es válido incluir la credencial
directamente dentro de `ConnectionStrings:SecurityDatabase` si se prefiere no
usar el secreto separado (por ejemplo, en un entorno con un *secret manager*
que ya provee la cadena completa).

Variables de entorno equivalentes (para contenedores u orquestadores):

```bash
export ConnectionStrings__SecurityDatabase="Server=localhost,1433;Database=DEMO;User Id=sa;TrustServerCertificate=True;"
export Db__SqlAuthSecret="<tu-password>"
```

## 4. Base de datos

Ejecutar en orden contra la instancia de SQL Server:

1. `sql/01_schema_seguridad.sql` — crea el esquema `seg` y las tablas
   `seg.Usuario`, `seg.Perfil`, `seg.Opcion`, `seg.Accion`, `seg.PerfilOpcion`,
   `seg.PerfilAccion`, `seg.UsuarioOpcion`, `seg.UsuarioAccion`.
2. `sql/02_seed_data.sql` — datos de ejemplo (árbol de opciones, un perfil
   Administrador, acciones típicas, etc.).

### Suposiciones documentadas sobre el modelo de datos

El script conceptual entregado en el requerimiento usa el esquema `dbo` y
nombres en singular. Dado que el enunciado exige explícitamente que la
información viva en `seg.*` (`seg.Perfil`, `seg.Opcion`, etc.), se recreó la
misma estructura de columnas bajo el esquema `seg`. Además:

- **`seg.Usuario`** no estaba definida en el script conceptual, pero es
  referenciada por FKs desde `UsuarioOpcion`/`UsuarioAccion`. Se creó una
  versión mínima (`Id`, `Nombre`, `Login`, `Activo` + columnas de auditoría)
  únicamente para satisfacer la integridad referencial; el módulo real de
  usuarios/autenticación puede reemplazarla.
- **`IdCarga`** (presente en `PerfilOpcion`, `PerfilAccion`, `UsuarioOpcion`,
  `UsuarioAccion`) no tiene un propósito documentado en el script original;
  se asume que identifica el proceso/lote de carga que originó el registro,
  con valor por defecto `0`.
- Los nombres de columnas (`Id`, `IdPadre`, `IdOpcion`, `IdPerfil`,
  `IdUsuario`, `IdAccion`, `UsuarioRegistro`, `FechaRegistro`,
  `UsuarioModifica`, `FechaModifica`, `Activo`, `Visible`, `Orden`, `Ruta`,
  `Codigo`) se tomaron directamente del script conceptual provisto.

Estas suposiciones están además comentadas al inicio de
`sql/01_schema_seguridad.sql` y pueden ajustarse libremente si la base real
difiere.

## 5. Ejecutar localmente

```bash
dotnet restore
dotnet build
dotnet run --project src/Security.Api
```

Swagger UI queda disponible en `https://localhost:<puerto>/swagger` (o
`http://localhost:<puerto>/swagger`), mostrando todos los endpoints
documentados.

## 6. Ejecutar con Docker

```bash
cp .env.example .env
# Editar .env y definir SQL_SA_PASSWORD con una contraseña fuerte real.

docker compose up -d --build
```

- El servicio `sqlserver` expone SQL Server Express (imagen
  `mcr.microsoft.com/mssql/server:2022-latest` con `MSSQL_PID=Express`) y
  persiste sus datos en el volumen nombrado `security_sqlserver_data`.
- El servicio `api` corre `Security.Api` y se conecta a SQL Server usando el
  nombre del servicio de Docker Compose (`sqlserver`), **no** `localhost`.
- Antes del primer uso, ejecutar los scripts de `sql/` contra la base
  expuesta en `localhost:1433` (o el host que corresponda) usando la misma
  contraseña definida en `.env`.

> ⚠️ **Importante:** no ejecutar `docker compose down -v` como parte del
> flujo normal de desarrollo, ya que eliminaría el volumen
> `security_sqlserver_data` y con él todos los datos persistidos. Usar
> `docker compose down` (sin `-v`) o `docker compose stop` para detener los
> contenedores conservando los datos.

## 7. Manejo de errores y respuestas estándar

Todas las respuestas de la API siguen el modelo `ApiResponse<T>`:

```json
{
  "success": true,
  "message": "Operación realizada correctamente.",
  "data": { },
  "errors": [],
  "traceId": "..."
}
```

Las excepciones se gestionan de forma centralizada en
`GlobalExceptionMiddleware` (sin `try/catch` repetido en los controllers),
mapeando cada tipo a un código HTTP:

| Excepción              | HTTP |
|-------------------------|------|
| `ValidationException`   | 400  |
| `UnauthorizedException` | 401  |
| `ForbiddenException`    | 403  |
| `NotFoundException`     | 404  |
| `BusinessException`     | 422  |
| `DatabaseException`     | 500  |
| `Exception` (genérica)  | 500  |

Cada error registrado incluye timestamp, mensaje, stack trace, endpoint,
método HTTP, usuario (cuando está disponible vía `ICurrentUserService`) y
`traceId`/`CorrelationId`, mediante `ILogger` — sin acoplarse a una
herramienta específica, lo que permite integrar posteriormente Serilog,
Seq, Elasticsearch o Application Insights.

## 8. Autenticación

La estructura de DI y el pipeline de middleware ya incluyen
`AddAuthentication`/`AddAuthorization` y `UseAuthentication`/
`UseAuthorization`, listos para integrar el esquema de autenticación que
corresponda (JWT, cookies, etc.) sin reestructurar la solución.
`ICurrentUserService` ya expone el usuario autenticado a los handlers para
las columnas de auditoría (`UsuarioRegistro`/`UsuarioModifica`).

## 9. Pruebas

```bash
dotnet test
```

Las pruebas unitarias (`tests/Security.Tests`) cubren las reglas de negocio
principales: rechazo de perfiles con código duplicado, detección de ciclos al
reasignar el padre de una opción, restricciones de borrado de opciones con
dependencias, validación de que una acción sólo pueda asignarse a un perfil si
su opción ya está asignada, y validación de pertenencia acción→opción en el
guardado transaccional de la configuración completa de permisos.

## 10. Transacciones

Las operaciones que modifican múltiples tablas (por ejemplo, guardar toda la
configuración de permisos de un perfil o usuario) se ejecutan dentro de una
transacción SQL a través de `IUnitOfWork`, implementada con Dapper: se abre
una conexión y transacción, se ejecutan todas las escrituras, se hace
`COMMIT` al finalizar sin errores, o `ROLLBACK` automático ante cualquier
excepción.

## 11. Principales endpoints

```
GET    /api/perfiles
GET    /api/perfiles/{id}
POST   /api/perfiles
PUT    /api/perfiles/{id}
DELETE /api/perfiles/{id}
GET    /api/perfiles/{id}/permisos
PUT    /api/perfiles/{id}/permisos

GET    /api/opciones
GET    /api/opciones/arbol
GET    /api/opciones/{id}
GET    /api/opciones/{id}/hijos
GET    /api/opciones/{id}/acciones
POST   /api/opciones
PUT    /api/opciones/{id}
DELETE /api/opciones/{id}

GET    /api/acciones
POST   /api/acciones
PUT    /api/acciones/{id}
DELETE /api/acciones/{id}

GET    /api/usuarios/{idUsuario}/permisos
PUT    /api/usuarios/{idUsuario}/permisos
```

El listado completo, con esquemas de request/response, está disponible en
Swagger UI al ejecutar la API.

## 12. Frontend

Este backend está diseñado para ser consumido por una aplicación Angular 20 +
PrimeNG 20 que administre visualmente la estructura de seguridad mediante un
componente tipo árbol (Tree), tal como se describe en el requerimiento
original. El diseño de los endpoints de árbol (`/api/opciones/arbol`,
`/api/perfiles/{id}/permisos`, `/api/usuarios/{id}/permisos`) responde
directamente a ese caso de uso.
