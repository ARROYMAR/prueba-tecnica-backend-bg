-- Creacion de Tabla Orden de compras
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


