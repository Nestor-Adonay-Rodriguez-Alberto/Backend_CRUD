-- Script SQL para crear las tablas Empleado y Serie
-- Basado exactamente en las entidades del proyecto Backend_CRUD
-- Fecha: 28 de noviembre de 2025

USE [TU_BASE_DE_DATOS]; -- Reemplaza con el nombre de tu base de datos
GO

-- Tabla Empleados
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Empleados' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Empleados] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(MAX) NOT NULL,
        [Puesto] NVARCHAR(MAX) NOT NULL,
        [Contraseña] NVARCHAR(MAX) NOT NULL,
        CONSTRAINT [PK_Empleados] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'Tabla Empleados creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla Empleados ya existe.';
END
GO

-- Tabla Series
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Series' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Series] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Titulo] NVARCHAR(MAX) NOT NULL,
        [Temporadas] INT NOT NULL,
        CONSTRAINT [PK_Series] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'Tabla Series creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla Series ya existe.';
END
GO

-- Datos de prueba (opcional)
-- Empleados de ejemplo
INSERT INTO [dbo].[Empleados] ([Nombre], [Puesto], [Contraseña])
SELECT 'Juan Pérez', 'Desarrollador', 'password123'
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Empleados] WHERE [Nombre] = 'Juan Pérez');

INSERT INTO [dbo].[Empleados] ([Nombre], [Puesto], [Contraseña])
SELECT 'María García', 'Analista', 'password456'
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Empleados] WHERE [Nombre] = 'María García');

-- Series de ejemplo
INSERT INTO [dbo].[Series] ([Titulo], [Temporadas])
SELECT 'Breaking Bad', 5
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Series] WHERE [Titulo] = 'Breaking Bad');

INSERT INTO [dbo].[Series] ([Titulo], [Temporadas])
SELECT 'Stranger Things', 4
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Series] WHERE [Titulo] = 'Stranger Things');

PRINT 'Script ejecutado completamente. Tablas y datos de prueba creados.';
GO

-- Verificación de las tablas creadas
SELECT 'Empleados' AS Tabla, COUNT(*) AS TotalRegistros FROM [dbo].[Empleados]
UNION ALL
SELECT 'Series' AS Tabla, COUNT(*) AS TotalRegistros FROM [dbo].[Series];
GO