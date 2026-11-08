-- Creacion de Base de Datos
IF DB_ID('CarritoDB') IS NULL
BEGIN
	CREATE DATABASE CarritoDB COLLATE Latin1_General_100_CI_AS_SC_UTF8;
	END
GO

use CarritoDB
go

/* ---------- Limpieza (orden inverso a las dependencias) ---------- */
DROP TABLE IF EXISTS dbo.OrdenDetalle;
DROP TABLE IF EXISTS dbo.Orden;
DROP TABLE IF EXISTS dbo.CarritoCompraDetalle;
DROP TABLE IF EXISTS dbo.CarritoCompra;
DROP TABLE IF EXISTS dbo.Producto;
DROP TABLE IF EXISTS dbo.Categoria;
DROP TABLE IF EXISTS dbo.Usuario;
GO

-- 1.- Creacion de Tabla de Usuario e insercion de datos
USE CarritoDB
GO

DROP TABLE IF EXISTS dbo.Usuario;
GO
BEGIN
CREATE TABLE dbo.Usuario (
    Id              INT IDENTITY(1,1) CONSTRAINT PK_Usuario PRIMARY KEY,
    Usuario         NVARCHAR(50)   NOT NULL,
    Email           NVARCHAR(150)  NOT NULL,
    Nombre          NVARCHAR(150)  NOT NULL,
    Password        NVARCHAR(500)  NOT NULL,
    Rol             NVARCHAR(20)   NOT NULL CONSTRAINT DF_Usuario_Rol DEFAULT 'Cliente',
    Estado          BIT            NOT NULL CONSTRAINT DF_Usuario_Estado DEFAULT 1,
    FechaCreacion   DATETIME2(0)   NOT NULL CONSTRAINT DF_Usuario_FechaCreacion DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_Users_Usuario UNIQUE (Usuario),
    CONSTRAINT UQ_Users_Email    UNIQUE (Email),
    CONSTRAINT CK_Users_Rol     CHECK (Rol IN ('Admin', 'Cliente'))
);
END
GO


-- 2.- Creacion de tabla Categoria
USE CarritoDB
GO

DROP TABLE IF EXISTS dbo.Categoria;
GO
CREATE TABLE dbo.Categoria (
    Id   INT IDENTITY(1,1) CONSTRAINT PK_Categoria PRIMARY KEY,
    Descripcion NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Categoria_Nombre UNIQUE (Descripcion)
);
GO


-- 3.- Creacion de Tabla Producto
USE CarritoDB
GO

DROP TABLE IF EXISTS dbo.Producto;
GO
CREATE TABLE dbo.Producto (
    Id              INT IDENTITY(1,1) CONSTRAINT PK_Producto PRIMARY KEY,
    Codigo          NVARCHAR(30)   NOT NULL,
    Producto        NVARCHAR(150)  NOT NULL,
    Descripcion     NVARCHAR(500)  NULL,
    CategoriaId     INT            NOT NULL,
    Precio          DECIMAL(18,2)  NOT NULL,
    Stock           INT            NOT NULL,
    Estado          BIT            NOT NULL CONSTRAINT DF_Producto_Estado DEFAULT 1,
    CONSTRAINT UQ_Producto_Codigo UNIQUE (Codigo),
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (CategoriaId) REFERENCES dbo.Categoria(Id),
    CONSTRAINT CK_Producto_Precio CHECK (Precio >= 0),
    CONSTRAINT CK_Producto_Stock CHECK (Stock >= 0)
);
GO

CREATE INDEX IX_Products_Name     ON dbo.Producto (Producto);
CREATE INDEX IX_Products_Category ON dbo.Producto (CategoriaId);



-- 4.- Creacion de Tabla Cabecera del carrito de compras
USE CarritoDB
GO

DROP TABLE IF EXISTS dbo.CarritoCompra;
GO
CREATE TABLE dbo.CarritoCompra (
    Id        INT IDENTITY(1,1) CONSTRAINT PK_CarritoCompra PRIMARY KEY,
    UsuarioId    INT          NOT NULL,
    FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_FechaCreacion DEFAULT SYSUTCDATETIME(),
    FechaModificacion DATETIME2(0) NOT NULL CONSTRAINT DF_FechaModificacion DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_Carrito_Usuario UNIQUE (UsuarioId),
    CONSTRAINT FK_Carrito_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario(Id)
);
GO

-- 5.- Creacion de Tabla Detalle del carrito de compras
USE CarritoDB
GO

DROP TABLE IF EXISTS dbo.CarritoCompraDetalle;
GO
CREATE TABLE dbo.CarritoCompraDetalle (
    Id        INT IDENTITY(1,1) CONSTRAINT PK_CarritoCompraDetalle PRIMARY KEY,
    CarritoId    INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad  INT NOT NULL,
    CONSTRAINT UQ_CarritoCompraDetalle_Producto UNIQUE (CarritoId, ProductoId),
    CONSTRAINT FK_CarritoCompraDetalle_CarritoCompra    FOREIGN KEY (CarritoId)    REFERENCES dbo.CarritoCompra(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CarritoCompraDetalle_Producto FOREIGN KEY (ProductoId) REFERENCES dbo.Producto(Id),
    CONSTRAINT CK_CarritoCompraDetalle_Cantidad CHECK (Cantidad > 0)
);
GO


-- 6.- Creacion de Tabla Orden de compras
USE CarritoDB
GO

DROP TABLE IF EXISTS dbo.Orden;
GO
CREATE TABLE dbo.Orden (
    Id                  INT IDENTITY(1,1) CONSTRAINT PK_Orden PRIMARY KEY,
    UsuarioId           INT           NOT NULL,
    OrdenCreacion       DATETIME2(0)  NOT NULL CONSTRAINT DF_OrdenCreacion DEFAULT SYSUTCDATETIME(),
    Subtotal            DECIMAL(18,2) NOT NULL,
    PorcDescuento       DECIMAL(5,2)  NOT NULL CONSTRAINT DF_PorcDescuento DEFAULT 0,
    Descuento           DECIMAL(18,2) NOT NULL CONSTRAINT DF_Descuento DEFAULT 0,
    Total               DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Orden_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario(Id)
);
GO


-- 7.- Creacion de Tabla Orden de compras Detalle
USE CarritoDB
GO

DROP TABLE IF EXISTS dbo.OrdeDetalle;
GO

CREATE TABLE dbo.OrdenDetalle (
    Id          INT IDENTITY(1,1) CONSTRAINT PK_OrdenDetalle PRIMARY KEY,
    OrdenId     INT           NOT NULL,
    ProductoId   INT           NOT NULL,
    ProductoCodigo NVARCHAR(30)  NOT NULL,
    ProductoNombre NVARCHAR(150) NOT NULL,
    Precio   DECIMAL(18,2) NOT NULL,
    Cantidad    INT           NOT NULL,
    Total   DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrdenDetalle_Orden   FOREIGN KEY (OrdenId)   REFERENCES dbo.Orden(Id),
    CONSTRAINT FK_OrdenDetalle_Producto FOREIGN KEY (ProductoId) REFERENCES dbo.Producto(Id),
    CONSTRAINT CK_OrdenDetalle_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_OrdenDetalle_Total     CHECK (Total = Precio * Cantidad)
);
GO

CREATE INDEX IX_OrdenDetalle_Orden ON dbo.OrdenDetalle (OrdenId);
GO


USE CarritoDB
GO

-- Se insertan datos del usuario
INSERT INTO dbo.Usuario (Usuario, Email, Nombre, Password, Rol) VALUES
(N'admin',   N'administrator',   N'Administrador del Sistema',
 N'AQAAAAIAAYagAAAAEHJMzFbGoLv7x7H3yJr+W5StZe5ciG6Zg7Wn0bRkWmlA+XAwraOg0g3ohnYnwfLTsA==', 'Admin'),
(N'cliente', N'cliente@carrito.test', N'Cliente',
 N'AQAAAAIAAYagAAAAEMr+TXubOBzR8n92mjAz+RLxjfET3PXs4utZlfghvgYDIgmjDHtVrXZ/dt1RXCj9HA==', 'Cliente');

-- Se insertan datos de categoria
INSERT INTO dbo.Categoria (Descripcion) VALUES
('Tecnología'), ('Hogar'), ('Deportes'), ('Libros'), ('Oficina');
GO

-- Se insertan datos del producto
INSERT INTO dbo.Producto (Codigo, Producto, Descripcion, CategoriaId, Precio, Stock) VALUES
(N'TEC-001', N'Mouse inalámbrico',        N'Mouse óptico 2.4 GHz con receptor USB',            1,  15.99, 50),
(N'TEC-002', N'Teclado mecánico',         N'Teclado mecánico retroiluminado, switches rojos',  1,  59.90, 25),
(N'TEC-003', N'Audífonos Bluetooth',      N'Audífonos con cancelación de ruido y micrófono',   1,  79.50, 15),
(N'TEC-004', N'Monitor 24 pulgadas',      N'Monitor Full HD IPS 75 Hz',                        1, 139.00,  8),
(N'TEC-005', N'Memoria USB 64 GB',        N'Memoria USB 3.0 de 64 GB',                         1,   8.75,100),
(N'HOG-001', N'Lámpara de escritorio LED',N'Lámpara regulable con puerto de carga USB',        2,  22.40, 30),
(N'HOG-002', N'Licuadora 600 W',          N'Licuadora con jarra de vidrio de 1.5 L',           2,  45.00, 12),
(N'HOG-003', N'Juego de sábanas queen',   N'Juego de sábanas de algodón, 4 piezas',            2,  34.99, 20),
(N'DEP-001', N'Balón de fútbol N.º 5',    N'Balón profesional de cuero sintético',             3,  24.50, 40),
(N'DEP-002', N'Mancuernas 5 kg (par)',    N'Par de mancuernas recubiertas de vinilo',          3,  29.90, 18),
(N'DEP-003', N'Colchoneta de yoga',       N'Colchoneta antideslizante de 6 mm',                3,  18.00, 35),
(N'LIB-001', N'Clean Code',               N'Manual de estilo para desarrollo ágil de software',4,  38.00, 10),
(N'LIB-002', N'El Principito',            N'Edición ilustrada',                                4,   9.50, 60),
(N'OFI-001', N'Cuaderno universitario',   N'Cuaderno de 100 hojas, cuadriculado',              5,   2.25,200),
(N'OFI-002', N'Silla ergonómica',         N'Silla de oficina con soporte lumbar',              5, 120.00,  5),
(N'OFI-003', N'Resma de papel A4',        N'500 hojas, 75 g/m²',                               5,   4.80, 80),
(N'TEC-006', N'Cámara web HD (agotada)',  N'Producto sin stock para pruebas',                  1,  29.00,  0);
GO