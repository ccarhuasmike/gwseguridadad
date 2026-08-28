/*
    Script de creación del esquema de seguridad (seg).

    ASUNCIONES / SUPOSICIONES (documentadas según lo solicitado):
    - El modelo conceptual entregado usa el esquema [dbo] y nombres en singular
      (Perfil, Opcion, Accion, PerfilOpcion, PerfilAccion, UsuarioOpcion, UsuarioAccion).
      Este script recrea la misma estructura de columnas bajo el esquema [seg],
      tal como pide el enunciado del backend (seg.Perfil, seg.Opcion, etc.).
    - La tabla [dbo].[Usuario] no fue incluida en el script original, pero es
      referenciada por FKs desde UsuarioOpcion/UsuarioAccion. Se crea aquí una
      versión mínima (Id, Nombre, Login, Activo + columnas de auditoría) sólo
      para satisfacer la integridad referencial; el módulo real de usuarios
      (autenticación, credenciales, etc.) puede vivir en otro esquema/servicio
      y reemplazar esta tabla sin afectar el resto del modelo.
    - Los nombres de columnas (Id, IdPadre, IdOpcion, IdPerfil, IdUsuario,
      IdAccion, IdCarga, UsuarioRegistro, FechaRegistro, UsuarioModifica,
      FechaModifica, Activo, Visible, Orden, Ruta, Codigo) se tomaron
      directamente del script conceptual provisto por el usuario.
    - IdCarga en PerfilOpcion/PerfilAccion/UsuarioOpcion/UsuarioAccion no se
      documenta su propósito en el modelo original; se asume que identifica el
      proceso/lote de carga que originó el registro y se mantiene con un valor
      por defecto de 0 cuando no aplique.
*/

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'seg')
BEGIN
    EXEC('CREATE SCHEMA seg');
END
GO

-- ============================================================================
-- seg.Usuario (mínimo, ver nota de asunciones arriba)
-- ============================================================================
IF OBJECT_ID('seg.Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE seg.Usuario
    (
        Id                  INT IDENTITY(1,1) NOT NULL,
        Nombre              VARCHAR(150)      NOT NULL,
        Login               VARCHAR(100)      NULL,
        Activo              BIT               NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),
        UsuarioRegistro     INT               NOT NULL CONSTRAINT DF_Usuario_UsuarioRegistro DEFAULT (0),
        FechaRegistro       DATETIME          NOT NULL CONSTRAINT DF_Usuario_FechaRegistro DEFAULT (GETDATE()),
        UsuarioModifica     INT               NULL,
        FechaModifica       DATETIME          NULL,
        CONSTRAINT PK_Usuario PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- ============================================================================
-- seg.Perfil
-- ============================================================================
IF OBJECT_ID('seg.Perfil', 'U') IS NULL
BEGIN
    CREATE TABLE seg.Perfil
    (
        Id                  INT IDENTITY(1,1) NOT NULL,
        Codigo              CHAR(10)          NOT NULL,
        Nombre              VARCHAR(100)      NOT NULL,
        Descripcion         VARCHAR(250)      NULL,
        Activo              BIT               NOT NULL CONSTRAINT DF_Perfil_Activo DEFAULT (1),
        UsuarioRegistro     INT               NOT NULL CONSTRAINT DF_Perfil_UsuarioRegistro DEFAULT (0),
        FechaRegistro       DATETIME          NOT NULL CONSTRAINT DF_Perfil_FechaRegistro DEFAULT (GETDATE()),
        UsuarioModifica     INT               NULL,
        FechaModifica       DATETIME          NULL,
        CONSTRAINT PK_Perfil PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT UQ_Perfil_Codigo UNIQUE (Codigo)
    );
END
GO

-- ============================================================================
-- seg.Opcion (recursiva vía IdPadre)
-- ============================================================================
IF OBJECT_ID('seg.Opcion', 'U') IS NULL
BEGIN
    CREATE TABLE seg.Opcion
    (
        Id                  INT IDENTITY(1,1) NOT NULL,
        IdPadre             INT               NULL,
        Nombre              VARCHAR(100)      NOT NULL,
        Descripcion         VARCHAR(250)      NOT NULL,
        Ruta                VARCHAR(250)      NULL,
        Orden               TINYINT           NOT NULL CONSTRAINT DF_Opcion_Orden DEFAULT (0),
        Visible             BIT               NOT NULL CONSTRAINT DF_Opcion_Visible DEFAULT (1),
        Activo              BIT               NOT NULL CONSTRAINT DF_Opcion_Activo DEFAULT (1),
        UsuarioRegistro     INT               NOT NULL CONSTRAINT DF_Opcion_UsuarioRegistro DEFAULT (0),
        FechaRegistro       DATETIME          NOT NULL CONSTRAINT DF_Opcion_FechaRegistro DEFAULT (GETDATE()),
        UsuarioModifica     INT               NULL,
        FechaModifica       DATETIME          NULL,
        CONSTRAINT PK_Opcion PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_Opcion_Opcion FOREIGN KEY (IdPadre) REFERENCES seg.Opcion (Id)
    );
END
GO

-- ============================================================================
-- seg.Accion (N acciones por Opcion)
-- ============================================================================
IF OBJECT_ID('seg.Accion', 'U') IS NULL
BEGIN
    CREATE TABLE seg.Accion
    (
        Id                  INT IDENTITY(1,1) NOT NULL,
        IdOpcion            INT               NOT NULL,
        Nombre              VARCHAR(100)      NOT NULL,
        Descripcion         VARCHAR(250)      NULL,
        Activo              BIT               NOT NULL CONSTRAINT DF_Accion_Activo DEFAULT (1),
        UsuarioRegistro     INT               NOT NULL CONSTRAINT DF_Accion_UsuarioRegistro DEFAULT (0),
        FechaRegistro       DATETIME          NOT NULL CONSTRAINT DF_Accion_FechaRegistro DEFAULT (GETDATE()),
        UsuarioModifica     INT               NULL,
        FechaModifica       DATETIME          NULL,
        CONSTRAINT PK_Accion PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_Opcion_Accion FOREIGN KEY (IdOpcion) REFERENCES seg.Opcion (Id)
    );
END
GO

-- ============================================================================
-- seg.PerfilOpcion (N:M Perfil <-> Opcion)
-- ============================================================================
IF OBJECT_ID('seg.PerfilOpcion', 'U') IS NULL
BEGIN
    CREATE TABLE seg.PerfilOpcion
    (
        IdPerfil            INT      NOT NULL,
        IdOpcion            INT      NOT NULL,
        IdCarga             INT      NOT NULL CONSTRAINT DF_PerfilOpcion_IdCarga DEFAULT (0),
        UsuarioRegistro     INT      NOT NULL CONSTRAINT DF_PerfilOpcion_UsuarioRegistro DEFAULT (0),
        FechaRegistro       DATETIME NOT NULL CONSTRAINT DF_PerfilOpcion_FechaRegistro DEFAULT (GETDATE()),
        CONSTRAINT PK_PerfilOpcion PRIMARY KEY CLUSTERED (IdPerfil ASC, IdOpcion ASC),
        CONSTRAINT FK_Perfil_PerfilOpcion FOREIGN KEY (IdPerfil) REFERENCES seg.Perfil (Id),
        CONSTRAINT FK_Opcion_PerfilOpcion FOREIGN KEY (IdOpcion) REFERENCES seg.Opcion (Id)
    );
END
GO

-- ============================================================================
-- seg.PerfilAccion (N:M Perfil <-> Accion)
-- ============================================================================
IF OBJECT_ID('seg.PerfilAccion', 'U') IS NULL
BEGIN
    CREATE TABLE seg.PerfilAccion
    (
        IdPerfil            INT      NOT NULL,
        IdAccion            INT      NOT NULL,
        IdCarga             INT      NOT NULL CONSTRAINT DF_PerfilAccion_IdCarga DEFAULT (0),
        UsuarioRegistro     INT      NOT NULL CONSTRAINT DF_PerfilAccion_UsuarioRegistro DEFAULT (0),
        FechaRegistro       DATETIME NOT NULL CONSTRAINT DF_PerfilAccion_FechaRegistro DEFAULT (GETDATE()),
        CONSTRAINT PK_PerfilAccion PRIMARY KEY CLUSTERED (IdPerfil ASC, IdAccion ASC),
        CONSTRAINT FK_Perfil_PerfilAccion FOREIGN KEY (IdPerfil) REFERENCES seg.Perfil (Id),
        CONSTRAINT FK_Accion_PerfilAccion FOREIGN KEY (IdAccion) REFERENCES seg.Accion (Id)
    );
END
GO

-- ============================================================================
-- seg.UsuarioOpcion (N:M Usuario <-> Opcion)
-- ============================================================================
IF OBJECT_ID('seg.UsuarioOpcion', 'U') IS NULL
BEGIN
    CREATE TABLE seg.UsuarioOpcion
    (
        IdUsuario           INT      NOT NULL,
        IdOpcion            INT      NOT NULL,
        IdCarga             INT      NOT NULL CONSTRAINT DF_UsuarioOpcion_IdCarga DEFAULT (0),
        UsuarioRegistro     INT      NOT NULL CONSTRAINT DF_UsuarioOpcion_UsuarioRegistro DEFAULT (0),
        FechaRegistro       DATETIME NOT NULL CONSTRAINT DF_UsuarioOpcion_FechaRegistro DEFAULT (GETDATE()),
        CONSTRAINT PK_UsuarioOpcion PRIMARY KEY CLUSTERED (IdUsuario ASC, IdOpcion ASC),
        CONSTRAINT FK_Usuario_UsuarioOpcion FOREIGN KEY (IdUsuario) REFERENCES seg.Usuario (Id),
        CONSTRAINT FK_Opcion_UsuarioOpcion FOREIGN KEY (IdOpcion) REFERENCES seg.Opcion (Id)
    );
END
GO

-- ============================================================================
-- seg.UsuarioAccion (N:M Usuario <-> Accion)
-- ============================================================================
IF OBJECT_ID('seg.UsuarioAccion', 'U') IS NULL
BEGIN
    CREATE TABLE seg.UsuarioAccion
    (
        IdAccion            INT      NOT NULL,
        IdUsuario           INT      NOT NULL,
        IdCarga             INT      NOT NULL CONSTRAINT DF_UsuarioAccion_IdCarga DEFAULT (0),
        UsuarioRegistro     INT      NOT NULL CONSTRAINT DF_UsuarioAccion_UsuarioRegistro DEFAULT (0),
        FechaRegistro       DATETIME NOT NULL CONSTRAINT DF_UsuarioAccion_FechaRegistro DEFAULT (GETDATE()),
        CONSTRAINT PK_UsuarioAccion PRIMARY KEY CLUSTERED (IdAccion ASC, IdUsuario ASC),
        CONSTRAINT FK_Accion_UsuarioAccion FOREIGN KEY (IdAccion) REFERENCES seg.Accion (Id),
        CONSTRAINT FK_Usuario_UsuarioAccion FOREIGN KEY (IdUsuario) REFERENCES seg.Usuario (Id)
    );
END
GO
