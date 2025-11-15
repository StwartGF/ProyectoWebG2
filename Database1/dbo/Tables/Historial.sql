CREATE TABLE [dbo].[Historial] (
    [IdHistorial]        INT            IDENTITY (1, 1) NOT NULL,
    [IdUsuario]          INT            NULL,
    [IdCurso]            INT            NULL,
    [Fecha_Inicio]       DATE           NULL,
    [Fecha_Finalizacion] DATE           NULL,
    [Calificacion]       DECIMAL (3, 2) NULL,
    [Estado_Curso]       NVARCHAR (20)  NULL,
    PRIMARY KEY CLUSTERED ([IdHistorial] ASC),
    FOREIGN KEY ([IdCurso]) REFERENCES [dbo].[Curso] ([IdCurso]),
    FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuario] ([IdUsuario])
);

