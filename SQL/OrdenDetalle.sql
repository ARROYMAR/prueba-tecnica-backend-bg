-- Creacion de Tabla Orden de compras Detalle
USE CarritoDB
GO

DROP TABLE IF EXISTS dbo.OrdeDetalle;
GO

CREATE TABLE dbo.OrdenDetalle (
    Id          INT IDENTITY(1,1) CONSTRAINT PK_OrdenDetalle PRIMARY KEY,
    OrdenId     INT           NOT NULL,
    ProductoId   INT           NOT NULL,
    ProductoCodigo NVARCHAR(30)  NOT NULL,
    Product0Nombre NVARCHAR(150) NOT NULL,
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
