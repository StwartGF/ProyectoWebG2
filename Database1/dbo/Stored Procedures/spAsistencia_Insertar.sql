CREATE PROCEDURE spAsistencia_Insertar
    @IdUsuario INT,
    @IdCurso INT,
    @Fecha DATE,
    @EstadoAsistencia VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Asistencia (IdUsuario, IdCurso, Fecha, EstadoAsistencia)
    VALUES (@IdUsuario, @IdCurso, @Fecha, @EstadoAsistencia);

    SELECT SCOPE_IDENTITY() AS IdAsistencia;
END;