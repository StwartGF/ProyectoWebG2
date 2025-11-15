CREATE TABLE [dbo].[Instructor] (
    [IdInstructor]     INT            IDENTITY (1, 1) NOT NULL,
    [Codigo]           NVARCHAR (40)  NULL,
    [NombreInstructor] NVARCHAR (80)  NULL,
    [Contrasena]       NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([IdInstructor] ASC),
    UNIQUE NONCLUSTERED ([Codigo] ASC)
);

