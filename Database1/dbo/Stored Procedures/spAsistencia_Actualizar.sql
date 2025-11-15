CREATE PROCEDURE spAsistencia_Actualizar
    @IdAsistencia INT,
    @IdUsuario INT,
    @IdCurso INT,
    @Fecha DATE,
    @EstadoAsistencia VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Asistencia
    SET 
        IdUsuario = @IdUsuario,
        IdCurso = @IdCurso,
        Fecha = @Fecha,
        EstadoAsistencia = @EstadoAsistencia
    WHERE IdAsistencia = @IdAsistencia;
END;