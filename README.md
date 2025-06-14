# WebApiTaller

API RESTful para la gestión de usuarios, talleres, vehículos, componentes, órdenes de mantenimiento y facturas en un entorno de taller mecánico.  
Desarrollada en **.NET 8**, utiliza **MongoDB** como base de datos y autenticación JWT.

---

## Características principales

- **Gestión de usuarios** (registro, consulta, autenticación, usuarios de tipo 1 según su JWT)
- **Gestión de talleres** (asociados a un usuario de tipo 2 según su JWT)
- **Gestión de vehículos** (asociados a usuarios)
- **Gestión de componentes** y ensamblajes
- **Órdenes de mantenimiento** y **facturación**
- **Autenticación y autorización JWT**
- **Filtros y validaciones** en endpoints
- **DTOs** para separación de modelos de dominio y datos expuestos

---

## Tecnologías utilizadas

- .NET 8
- MongoDB
- JWT (Json Web Token)
- Swagger (OpenAPI)
- Entity Framework Core (para compatibilidad, aunque se usa la extensión para MongoDB)
- Swashbuckle (documentación Swagger)

---

## Estructura de carpetas


```
WebApiTaller/
│
├── Controllers/         // Controladores de la API
├── Models/              // Modelos de dominio y DTOs
│   ├── DTO/             // Data Transfer Objects
│
├── Program.cs           // Configuración principal de la API
├── WebApiTaller.csproj  // Proyecto principal
└── Dockerfile           // Dockerfile para despliegue

```

---

## Configuración y ejecución

1. **Clona el repositorio**
2. **Configura las variables de entorno** (cadena de conexión a MongoDB, claves JWT, etc.)
3. **Ejecuta la API**  
   
```
   dotnet run --project WebApiTaller/WebApiTaller.csproj
   
```
4. **Accede a Swagger** en `https://localhost:puerto/swagger` para probar los endpoints.

### Docker


```
docker build -f WebApiTaller/Dockerfile -t webapitaller .
docker run -p 8080:8080 webapitaller

```

---

## Endpoints principales

### Usuarios

- `GET /api/users`  
  Listado de usuarios (filtros: name, surname, userId)
- `GET /api/users/{id}`  
  Detalle de usuario (solo el propio usuario puede acceder)
- `POST /api/users`  
  Crear usuario (rechaza si ya existe userId)
- `DELETE /api/users/{id}`  
  Elimina usuario (solo el propio usuario)

### Talleres

- `GET /api/workshops`  
  Listado de talleres (filtros: userId, location, speciality, name)
- `GET /api/workshops/{id}`  
  Detalle de taller (solo el propio taller puede acceder)
- `POST /api/workshops`  
  Crear taller (rechaza si ya existe para ese userId)
- `DELETE /api/workshops/{id}`  
  Elimina taller (solo el propio usuario)

### Vehículos, Componentes, Órdenes y Facturas

- Estructura similar, usando DTOs y validaciones análogas.

---

## Seguridad

- **JWT**: Todos los endpoints requieren autenticación.
- **Autorización**: Los usuarios solo pueden acceder a sus propios recursos.
- **Validaciones**: No se permite duplicidad de userId en usuarios ni talleres.

---

## Notas

- Los modelos expuestos en los endpoints usan DTOs para evitar exponer información sensible.
- El proyecto está preparado para despliegue en Docker.
