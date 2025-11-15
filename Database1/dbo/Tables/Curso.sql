CREATE TABLE [dbo].[Curso] (
    [IdCurso]             INT            IDENTITY (1, 1) NOT NULL,
    [NombreCurso]         NVARCHAR (100) NULL,
    [FechaDeInicio]       DATE           NULL,
    [FechaDeFinalizacion] DATE           NULL,
    [DuracionCurso]       INT            NULL,
    [CuposDisponibles]    INT            NULL,
    [Categoria]           NVARCHAR (50)  NULL,
    [Modalidad]           NVARCHAR (30)  NULL,
    [IdInstructor]        INT            NULL,
    PRIMARY KEY CLUSTERED ([IdCurso] ASC),
    FOREIGN KEY ([IdInstructor]) REFERENCES [dbo].[Instructor] ([IdInstructor])
);

