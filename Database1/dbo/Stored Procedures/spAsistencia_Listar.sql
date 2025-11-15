CREATE PROCEDURE spAsistencia_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        IdAsistencia,
        IdUsuario,
        IdCurso,
        Fecha,
        EstadoAsistencia
    FROM Asistencia;
END;