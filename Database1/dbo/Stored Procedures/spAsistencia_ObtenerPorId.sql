CREATE PROCEDURE spAsistencia_ObtenerPorId
    @IdAsistencia INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        IdAsistencia,
        IdUsuario,
        IdCurso,
        Fecha,
        EstadoAsistencia
    FROM Asistencia
    WHERE IdAsistencia = @IdAsistencia;
END;