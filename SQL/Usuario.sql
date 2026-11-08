-- Creacion de Tabla de Usuario e insercion de datos
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

