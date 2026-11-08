--Creacion de tabla Categoria
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
