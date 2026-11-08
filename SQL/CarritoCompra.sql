-- Creacion de Tabla Cabecera del carrito de compras
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