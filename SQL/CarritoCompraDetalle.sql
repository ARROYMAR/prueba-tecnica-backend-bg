-- Creacion de Tabla Detalle del carrito de compras
USE CarritoDB
GO

DROP TABLE IF EXISTS dbo.CarritoCompraDetalle;
GO
CREATE TABLE dbo.CarritoCompraDetalle (
    Id        INT IDENTITY(1,1) CONSTRAINT PK_CarritoCompraDetalle PRIMARY KEY,
    CarritotId    INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad  INT NOT NULL,
    CONSTRAINT UQ_CarritoCompraDetalle_Producto UNIQUE (CarritotId, ProductoId),
    CONSTRAINT FK_CarritoCompraDetalle_CarritoCompra    FOREIGN KEY (CarritotId)    REFERENCES dbo.CarritoCompra(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CarritoCompraDetalle_Producto FOREIGN KEY (ProductoId) REFERENCES dbo.Producto(Id),
    CONSTRAINT CK_CarritoCompraDetalle_Cantidad CHECK (Cantidad > 0)
);
GO
