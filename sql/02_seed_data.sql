/*
    Datos de ejemplo (seed) para pruebas manuales / demo, alineados con los
    ejemplos del enunciado (Sistema > Seguridad/Reportes/Configuración,
    Usuarios > Crear/Consultar/Editar/Eliminar, perfil Administrador, etc.).
*/

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM seg.Usuario WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT seg.Usuario ON;
    INSERT INTO seg.Usuario (Id, Nombre, Login, Activo, UsuarioRegistro)
    VALUES (1, 'Juan Pérez', 'jperez', 1, 0);
    SET IDENTITY_INSERT seg.Usuario OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM seg.Perfil WHERE Codigo = 'ADMIN')
BEGIN
    INSERT INTO seg.Perfil (Codigo, Nombre, Descripcion, Activo, UsuarioRegistro)
    VALUES ('ADMIN', 'Administrador', 'Perfil con acceso total al sistema.', 1, 0);
END
GO

DECLARE @IdSeguridad INT, @IdUsuarios INT, @IdPerfiles INT, @IdOpciones INT,
        @IdReportes INT, @IdReporte1 INT, @IdReporte2 INT, @IdConfiguracion INT;

IF NOT EXISTS (SELECT 1 FROM seg.Opcion WHERE Nombre = 'Seguridad' AND IdPadre IS NULL)
BEGIN
    INSERT INTO seg.Opcion (IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo, UsuarioRegistro)
    VALUES (NULL, 'Seguridad', 'Módulo de seguridad y permisos.', '/seguridad', 1, 1, 1, 0);
    SET @IdSeguridad = SCOPE_IDENTITY();

    INSERT INTO seg.Opcion (IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo, UsuarioRegistro)
    VALUES (@IdSeguridad, 'Usuarios', 'Administración de usuarios.', '/seguridad/usuarios', 1, 1, 1, 0);
    SET @IdUsuarios = SCOPE_IDENTITY();

    INSERT INTO seg.Opcion (IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo, UsuarioRegistro)
    VALUES (@IdSeguridad, 'Perfiles', 'Administración de perfiles.', '/seguridad/perfiles', 2, 1, 1, 0);
    SET @IdPerfiles = SCOPE_IDENTITY();

    INSERT INTO seg.Opcion (IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo, UsuarioRegistro)
    VALUES (@IdSeguridad, 'Opciones', 'Administración del árbol de opciones.', '/seguridad/opciones', 3, 1, 1, 0);
    SET @IdOpciones = SCOPE_IDENTITY();

    INSERT INTO seg.Opcion (IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo, UsuarioRegistro)
    VALUES (NULL, 'Reportes', 'Módulo de reportes.', '/reportes', 2, 1, 1, 0);
    SET @IdReportes = SCOPE_IDENTITY();

    INSERT INTO seg.Opcion (IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo, UsuarioRegistro)
    VALUES (@IdReportes, 'Reporte 1', 'Primer reporte de ejemplo.', '/reportes/1', 1, 1, 1, 0);
    SET @IdReporte1 = SCOPE_IDENTITY();

    INSERT INTO seg.Opcion (IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo, UsuarioRegistro)
    VALUES (@IdReportes, 'Reporte 2', 'Segundo reporte de ejemplo.', '/reportes/2', 2, 1, 1, 0);
    SET @IdReporte2 = SCOPE_IDENTITY();

    INSERT INTO seg.Opcion (IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo, UsuarioRegistro)
    VALUES (NULL, 'Configuración', 'Configuración general del sistema.', '/configuracion', 3, 1, 1, 0);
    SET @IdConfiguracion = SCOPE_IDENTITY();

    INSERT INTO seg.Accion (IdOpcion, Nombre, Descripcion, Activo, UsuarioRegistro)
    VALUES
        (@IdUsuarios, 'Crear', 'Crear usuarios.', 1, 0),
        (@IdUsuarios, 'Consultar', 'Consultar usuarios.', 1, 0),
        (@IdUsuarios, 'Editar', 'Editar usuarios.', 1, 0),
        (@IdUsuarios, 'Eliminar', 'Eliminar usuarios.', 1, 0),
        (@IdReportes, 'Consultar', 'Consultar reportes.', 1, 0),
        (@IdReportes, 'Exportar', 'Exportar reportes.', 1, 0);
END
GO
