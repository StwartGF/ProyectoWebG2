CREATE TABLE [dbo].[Asistencia] (
    [IdAsistencia]     INT           IDENTITY (1, 1) NOT NULL,
    [IdUsuario]        INT           NULL,
    [IdCurso]          INT           NULL,
    [Fecha]            DATE          NULL,
    [EstadoAsistencia] NVARCHAR (15) NULL,
    PRIMARY KEY CLUSTERED ([IdAsistencia] ASC),
    FOREIGN KEY ([IdCurso]) REFERENCES [dbo].[Curso] ([IdCurso]),
    FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuario] ([IdUsuario])
);

