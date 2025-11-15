CREATE PROCEDURE dbo.Registro
    @Cedula NVARCHAR(20),
    @Nombre NVARCHAR(100),
    @Apellidos NVARCHAR(150),
    @Telefono NVARCHAR(30) = NULL,
    @Correo NVARCHAR(200),
    @ContrasenaHash NVARCHAR(500),
    @IdRol INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Cedula = @Cedula)
    BEGIN
        SELECT -1; -- cédula duplicada
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = @Correo)
    BEGIN
        SELECT -2; -- correo duplicado
        RETURN;
    END

    INSERT INTO dbo.Usuario (Cedula, Nombre, Apellidos, Telefono, Correo, ContrasenaHash, IdRol)
    VALUES (@Cedula, @Nombre, @Apellidos, @Telefono, @Correo, @ContrasenaHash, @IdRol);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdUsuarioCreado;
END