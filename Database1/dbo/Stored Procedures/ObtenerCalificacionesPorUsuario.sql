--Procedimiento almacenado para ver las calificaciones
CREATE   PROCEDURE dbo.ObtenerCalificacionesPorUsuario
    @IdUsuario INT
AS
BEGIN
    SELECT 
        h.IdCurso,
        c.NombreCurso,
        h.Calificacion,
        h.Fecha_Inicio,
        h.Fecha_Finalizacion,
        h.Estado_Curso
    FROM Historial h
    INNER JOIN Curso c ON h.IdCurso = c.IdCurso
    WHERE h.IdUsuario = @IdUsuario
END