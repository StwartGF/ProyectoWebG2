CREATE TABLE [dbo].[Matricula] (
    [IdMatricula]    INT           IDENTITY (1, 1) NOT NULL,
    [IdUsuario]      INT           NULL,
    [IdCurso]        INT           NULL,
    [FechaMatricula] DATE          NULL,
    [Estado]         NVARCHAR (20) NULL,
    PRIMARY KEY CLUSTERED ([IdMatricula] ASC),
    FOREIGN KEY ([IdCurso]) REFERENCES [dbo].[Curso] ([IdCurso]),
    FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuario] ([IdUsuario])
);

