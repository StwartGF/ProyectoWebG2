CREATE TABLE [dbo].[Usuario] (
    [IdUsuario]      INT            IDENTITY (1, 1) NOT NULL,
    [Cedula]         NVARCHAR (20)  NOT NULL,
    [Nombre]         NVARCHAR (100) NOT NULL,
    [Apellidos]      NVARCHAR (150) NOT NULL,
    [Telefono]       NVARCHAR (30)  NULL,
    [Correo]         NVARCHAR (200) NOT NULL,
    [ContrasenaHash] NVARCHAR (500) NOT NULL,
    [IdRol]          INT            NULL,
    [FechaCreacion]  DATETIME2 (3)  CONSTRAINT [DF_Usuario_FechaCreacion] DEFAULT (sysutcdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([IdUsuario] ASC),
    CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY ([IdRol]) REFERENCES [dbo].[Rol] ([IdRol]),
    UNIQUE NONCLUSTERED ([Cedula] ASC),
    UNIQUE NONCLUSTERED ([Correo] ASC)
);

