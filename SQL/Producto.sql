-- Creacion de Tabla Producto
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

