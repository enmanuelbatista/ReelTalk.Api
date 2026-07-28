# ReelTalk.Api

API REST para un catálogo de películas construida con **.NET 7**, **Entity Framework Core** y **SQL Server**. Permite registrar, modificar, eliminar y consultar películas, importarlas automáticamente desde [OMDb API](https://www.omdbapi.com/) usando el ID de IMDb, y protege las operaciones de escritura con autenticación **JWT (JSON Web Tokens)**.

## Tabla de contenidos

- [Tecnologías](#tecnologías)
- [Características](#características)
- [Requisitos previos](#requisitos-previos)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Migraciones de base de datos](#migraciones-de-base-de-datos)
- [Ejecutar el proyecto](#ejecutar-el-proyecto)
- [Autenticación](#autenticación)
- [Endpoints](#endpoints)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Próximas mejoras](#próximas-mejoras)

## Tecnologías

- **.NET 7** / ASP.NET Core Web API
- **Entity Framework Core 7** (SQL Server)
- **JWT Bearer Authentication** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **ASP.NET Core Identity** — hasheo de contraseñas (`IPasswordHasher<T>`)
- **Swagger** — documentación interactiva de la API
- **OMDb API** — importación de datos de películas desde una fuente externa

## Características

- CRUD completo de películas (crear, consultar, actualizar, eliminar)
- Importación automática de películas por IMDb ID consumiendo la API de OMDb
- Registro y autenticación de usuarios con contraseñas hasheadas (nunca en texto plano)
- Tokens JWT firmados con expiración configurable
- Protección selectiva de endpoints: las consultas (`GET`) son públicas, las operaciones de escritura (`POST`, `PUT`, `DELETE`) requieren un token válido
- Manejo centralizado de excepciones con middleware global
- Configuración sensible (cadena de conexión, API keys, llave JWT) gestionada con **User Secrets**, fuera del control de versiones

## Requisitos previos

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express o una instancia completa)
- Una API Key gratuita de [OMDb API](https://www.omdbapi.com/apikey.aspx) (opcional, solo necesaria para el endpoint de importación)
- Visual Studio 2022 o cualquier editor compatible con .NET

## Instalación

```bash
git clone https://github.com/enmanuelbatista/ReelTalk.Api.git
cd ReelTalk.Api
dotnet restore
```

## Configuración

El proyecto usa [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) para mantener fuera del repositorio cualquier dato sensible. `appsettings.json` solo define la **estructura** de la configuración, con los valores reales vacíos:

```json
{
  "ConnectionStrings": {
    "CadenaSQL": ""
  },
  "OmdbApi": {
    "ApiKey": "",
    "BaseUrl": "http://www.omdbapi.com/"
  },
  "Jwt": {
    "Key": "",
    "Issuer": "ReelTalk.Api",
    "Audience": "ReelTalk.Api",
    "ExpiraEnMinutos": 60
  }
}
```

Configura tus propios valores localmente con la CLI de .NET, desde la carpeta del proyecto:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:CadenaSQL" "Server=TU_SERVIDOR;Database=ReelTalkDb;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet user-secrets set "OmdbApi:ApiKey" "tu-api-key-de-omdb"
dotnet user-secrets set "Jwt:Key" "una-clave-secreta-larga-y-unica-de-al-menos-32-caracteres"
```

> ⚠️ La `Jwt:Key` firma y valida todos los tokens emitidos por la API. Debe ser larga (32+ caracteres), aleatoria, y nunca debe subirse a un repositorio público.

## Migraciones de base de datos

Con la cadena de conexión ya configurada en User Secrets, aplica las migraciones para crear el esquema en SQL Server:

```bash
dotnet ef database update
```

Esto crea las tablas `Peliculas` y `Usuarios`. Si necesitas herramientas de EF Core:

```bash
dotnet tool install --global dotnet-ef
```

## Ejecutar el proyecto

```bash
dotnet run
```

Por defecto, la API queda disponible en `https://localhost:{puerto}` (el puerto exacto se define en `Properties/launchSettings.json`). En entorno de desarrollo, Swagger UI se abre automáticamente en `/swagger`, donde se pueden probar todos los endpoints.

## Autenticación

La API usa JWT Bearer. El flujo es:

1. **Registro** — `POST /api/Auth/registro` con un `username` y `password`. La contraseña se hashea antes de guardarse.
2. **Login** — `POST /api/Auth/login` con las mismas credenciales. La respuesta incluye un `token` JWT.
3. **Autorización** — en las siguientes peticiones a endpoints protegidos, incluir el token en el header:

```
Authorization: Bearer {tu_token}
```

En Swagger, esto se hace con el botón **Authorize** 🔒 pegando el token (con el prefijo `Bearer `).

El token expira según `Jwt:ExpiraEnMinutos` (60 minutos por defecto); pasado ese tiempo, hay que volver a iniciar sesión.

## Endpoints

### Auth

| Método | Ruta | Descripción | Requiere token |
|---|---|---|---|
| POST | `/api/Auth/registro` | Registra un nuevo usuario | No |
| POST | `/api/Auth/login` | Autentica y devuelve un JWT | No |

### Películas

| Método | Ruta | Descripción | Requiere token |
|---|---|---|---|
| GET | `/api/Peliculas` | Lista todas las películas | No |
| GET | `/api/Peliculas/{id}` | Obtiene una película por ID | No |
| POST | `/api/Peliculas` | Crea una nueva película | Sí |
| POST | `/api/Peliculas/importar/{imdbId}` | Importa una película desde OMDb por su IMDb ID | Sí |
| PUT | `/api/Peliculas/{id}` | Actualiza una película existente | Sí |
| DELETE | `/api/Peliculas/{id}` | Elimina una película | Sí |

## Estructura del proyecto

```
ReelTalk.Api/
├── Controllers/       # Endpoints de la API (Auth, Peliculas)
├── Data/               # DbContext de Entity Framework Core
├── DTOs/               # Objetos de transferencia de datos (Auth, OMDb)
├── Middlewares/        # Manejo global de excepciones
├── Migrations/         # Historial de migraciones de EF Core
├── Modelos/             # Entidades del dominio (Pelicula, Usuario)
├── Services/            # Lógica de negocio (OmdbService, TokenService)
├── appsettings.json    # Configuración base (sin secretos)
└── Program.cs           # Configuración de la app y el pipeline HTTP
```

## Próximas mejoras

- [ ] Migrar de .NET 7 (fuera de soporte) a .NET 8 (LTS)
- [ ] Roles y autorización granular (`[Authorize(Roles = "Admin")]`)
- [ ] Paginación en `GET /api/Peliculas`
- [ ] Refresh tokens para evitar reautenticación frecuente
- [ ] Pruebas unitarias e integración
- [ ] Documentación de errores/respuestas en Swagger con ejemplos
