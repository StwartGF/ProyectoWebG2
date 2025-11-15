CREATE TABLE [dbo].[Rol] (
    [IdRol]  INT           IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([IdRol] ASC),
    UNIQUE NONCLUSTERED ([Nombre] ASC)
);

