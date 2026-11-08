-- Creacion de Base de Datos
IF DB_ID('CarritoDB') IS NULL
BEGIN
	CREATE DATABASE CarritoDB COLLATE Latin1_General_100_CI_AS_SC_UTF8;
	END
GO

use CarritoDB
go

