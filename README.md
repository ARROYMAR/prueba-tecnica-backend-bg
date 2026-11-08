# CarritoApi — Backend del Carrito de Compras

API REST para un carrito de compras: inicio de sesión con JWT, catálogo de productos con búsqueda y filtros, carrito con validación de stock, descuento automático y generación de órdenes de compra.

## Tecnologías

- .NET 10 (ASP.NET Core Web API)
- Entity Framework Core 10 + SQL Server
- Autenticación JWT (Bearer)
- Swagger / OpenAPI
- `Password` de ASP.NET Core Identity para el hash de contraseñas

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- SQL Server 2019 o superior (local, Express o Docker)
- SQL Server Management Studio (SSMS) o Azure Data Studio

## Instalación y ejecución

### 1. Crear la base de datos

Abrir `database/CarritoDB.sql` en SSMS y ejecutarlo completo (F5). El script:

- crea la base `CarritoDB` (si no existe),
- borra y vuelve a crear las tablas en el orden correcto,
- inserta los datos semilla (usuarios, categorías y productos).

Se puede ejecutar varias veces sin problema.


### 2. Configurar la conexión

En `CarritoApi/appsettings.json`, ajustar la cadena de conexión al servidor local (el mismo nombre que se usa en SSMS):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CarritoDB;User Id=sa;Password=contraseñasa;TrustServerCertificate=True;"
  },
}
```

### 3. Ejecutar la API

cd CarritoApi
dotnet run
```

- API: http://localhost:5080
- Swagger: http://localhost:5080/swagger

Para probar endpoints protegidos en Swagger:

1. Ejecutar `POST /api/auth/login` con un usuario de prueba.
2. Copiar el `token` de la respuesta.
3. Pulsar **Authorize** y pegar el token.

## Usuarios de prueba

 Usuario  -  Contraseña  - Rol

`admin`   - `Admin123!`  - Admin
`cliente` - `Cliente123!`- Cliente

Las contraseñas se guardan como hash (PBKDF2-HMAC-SHA512, 100.000 iteraciones), nunca en texto plano.

## Configuración

 Clave - Descripción - Valor por defecto
 `ConnectionStrings:DefaultConnection` -| Conexión a SQL Server - `localhost` / `CarritoDB` 
`Jwt:Key` - Clave de firma del token (mínimo 32 caracteres) - definida en `appsettings.Development.json` 
 `Jwt:Issuer` / `Jwt:Audience` - Emisor y audiencia del token - `CarritoApi` / `CarritoFrontend` 
`Jwt:ExpirationMinutes` - Duración del token - `60` 
`Descuento:MontoMinimo` - Subtotal a partir del cual se aplica descuento - `100`
`Descuento:Porcentaje` - Porcentaje de descuento - `10` -
`Cors:AllowedOrigins` - Orígenes permitidos - `http://localhost:4200`

Cualquier valor se puede sobrescribir con variables de entorno, sin modificar los archivos:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=CarritoDB;User Id=sa;Password=TuClave;TrustServerCertificate=True;"
$env:Jwt__Key="una-clave-segura-de-al-menos-32-caracteres"
dotnet run
```

## Endpoints

Todos, excepto el login, requieren el encabezado `Authorization: Bearer {token}`.

### Autenticación

| Método | Ruta | Descripción | Respuestas |
| POST | `/api/auth/login` | Inicia sesión y devuelve el token JWT | 200, 400, 401 |

### Productos y categorías

| Método | Ruta | Descripción | Respuestas |
| GET | `/api/products` | Listado paginado con búsqueda y filtros | 200, 400, 401 |
| GET | `/api/products/{id}` | Detalle de un producto | 200, 404 |
| POST | `/api/products` | Crear producto (solo Admin) | 201, 400, 403, 409 |
| PUT | `/api/products/{id}` | Editar producto (solo Admin) | 200, 400, 403, 404, 409 |
| DELETE | `/api/products/{id}` | Desactivar producto, borrado lógico (solo Admin) | 204, 403, 404 |
| GET | `/api/categories` | Categorías para el filtro | 200 |

Parámetros de `GET /api/products` (todos opcionales):
| Parámetro | Descripción |
| `buscar` | Texto a buscar en nombre, código o categoría |
| `categoriaId` | Filtra por categoría |
| `precioMin` / `precioMax` | Rango de precio |
| `conStock` | `true` para mostrar solo productos con stock |
| `pagina` / `tamanoPagina` | Paginación (por defecto 1 y 12; máximo 100 por página) |

Ejemplo: `GET /api/products?buscar=mouse&conStock=true&pagina=1&tamanoPagina=12`

### Carrito

| Método | Ruta | Descripción | Respuestas |
| GET | `/api/cart` | Carrito con subtotal, descuento y total | 200 |
| POST | `/api/cart/items` | Agregar producto (o sumar cantidad) validando stock | 200, 400, 404, 409 |
| PUT | `/api/cart/items/{productoId}` | Cambiar la cantidad validando stock | 200, 400, 404, 409 |
| DELETE | `/api/cart/items/{productoId}` | Quitar un producto | 200, 404 |
| DELETE | `/api/cart` | Vaciar el carrito | 204 |

### Órdenes

| Método | Ruta | Descripción | Respuestas |
| POST | `/api/orders` | Finalizar la compra con el carrito actual | 201, 400, 409 |
| GET | `/api/orders` | Historial de compras del usuario | 200 |
| GET | `/api/orders/{id}` | Detalle de una compra | 200, 404 |

### Formato de errores

Los errores se devuelven como `ProblemDetails`:

json
{
  "status": 409,
  "title": "Stock insuficiente",
  "detail": "Stock insuficiente para 'Silla ergonómica'. Disponible: 5.",
  "codigo": "STOCK_INSUFICIENTE",
  "stockDisponible": 5
}


| Código | Cuándo |
| 400 | Datos inválidos o carrito vacío al comprar |
| 401 | Sin token, token vencido o credenciales incorrectas |
| 403 | Usuario sin el rol requerido (ej. Cliente intentando crear productos) |
| 404 | Producto u orden inexistente, o una orden de otro usuario |
| 409 | Stock insuficiente o código de producto duplicado |
| 500 | Error inesperado (se registra en el log sin exponer detalles) |

## Reglas de negocio
- **Stock:** no se puede agregar al carrito más unidades de las disponibles. Se valida al agregar, al cambiar la cantidad y otra vez al confirmar la compra.
- **Descuento:** si el subtotal es **mayor a $100**, se aplica un **10%**. La regla se configura en `appsettings.json`.
- **Compra:** genera la orden, descuenta el stock y vacía el carrito en una sola transacción.
- **Historial:** cada usuario solo puede ver sus propias órdenes.

## Estructura del proyecto
carrito-backend/
	database/
		CarritoDB.sql          Creación de la base, tablas y datos semilla
	CorregirColumnas.sql   Corrección de nombres de columnas (bases antiguas)
	CarritoApi/
		Controllers/   Endpoints HTTP: reciben DTOs, llaman al servicio y devuelven el código adecuado
		Services/      Lógica de negocio (Auth, Token, Producto, Carrito, Orden, Descuento)
		Data/          AppDbContext: mapeo de entidades a las tablas existentes
		Entities/      Una clase por tabla, con los mismos nombres de columnas
		DTOs/          Contratos de entrada y salida de la API
		Exceptions/    Excepciones de negocio que se traducen a 400 / 404 / 409
		Middleware/    Manejo centralizado de errores y logging
		Extensions/    Utilidades (obtener el usuario autenticado del token)
		Options/       Configuración tipada (Jwt, Descuento)
		Program.cs     Configuración de servicios, JWT, CORS y Swagger

### Nombres

Las entidades, propiedades y DTOs usan los mismos nombres que las tablas y columnas de la base de datos. Hay dos excepciones, porque C# no permite que una propiedad se llame igual que su clase:

- `Usuario.NombreUsuario` → columna `Usuario`
- `Producto.NombreProducto` → columna `Producto`

En los DTOs (JSON) sí se llaman `usuario` y `producto`. Las rutas se mantienen en inglés (`/api/products`, `/api/cart`, `/api/orders`) porque así las define el enunciado.

## Decisiones técnicas

- **Compra consistente ante concurrencia:** `POST /api/orders` se ejecuta dentro de una transacción. El stock se descuenta con un `UPDATE ... WHERE Stock >= cantidad` atómico (`ExecuteUpdateAsync`). Si algún producto no alcanza, se hace rollback y no queda ni la orden ni stock descontado. Así dos compras simultáneas no pueden dejar el stock en negativo.
- **Separación por capas:** los controladores solo manejan HTTP. La lógica está en servicios inyectados por interfaz, y las entidades nunca se exponen directamente, siempre a través de DTOs.
- **Regla de descuento en un solo lugar:** `DescuentoService` la usan tanto el carrito como la orden, así ambos calculan siempre igual.
- **Historial inmutable:** `OrdenDetalle` guarda el código, nombre y precio del producto al momento de la compra. Si luego cambia el precio, las órdenes anteriores no se alteran.
- **Borrado lógico de productos:** desactivar un producto pone `Estado = 0`, para no romper las órdenes que lo referencian.
- **Seguridad:** el login no revela si el usuario existe. Las órdenes ajenas responden 404 (no 403) para no confirmar que existen.
- **Base de datos por script:** el esquema se entrega como SQL. EF Core solo mapea, no se usan migraciones.
- **Fechas en UTC:** se guardan en UTC y se envían con `Z`, para que el frontend las muestre en la hora local del usuario.

## Problemas frecuentes

Problema - Solución
`Cannot open database "CarritoDB"`- Ejecutar `database/CarritoDB.sql` o revisar el nombre del servidor en la cadena de conexión
`Invalid column name 'CarritoId'` - La base se creó con los scripts originales: ejecutar `database/CorregirColumnas.sql`
Error de certificado SSL al conectar  Agregar `TrustServerCertificate=True;` a la cadena de conexión
El frontend recibe error de CORS - Verificar que `Cors:AllowedOrigins` incluya `http://localhost:4200`