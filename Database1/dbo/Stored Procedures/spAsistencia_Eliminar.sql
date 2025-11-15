CREATE PROCEDURE spAsistencia_Eliminar
    @IdAsistencia INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Asistencia
    WHERE IdAsistencia = @IdAsistencia;
END;