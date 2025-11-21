IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Correo] nvarchar(max) NOT NULL,
    [Contraseña] nvarchar(max) NOT NULL,
    [Rol] nvarchar(max) NOT NULL,
    [Telefono] nvarchar(max) NULL,
    [Posicion] nvarchar(max) NULL,
    [FotoPerfil] nvarchar(max) NULL,
    [Ubicacion] nvarchar(max) NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Canchas] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Tipo] nvarchar(max) NOT NULL,
    [Superficie] nvarchar(max) NOT NULL,
    [Estado] nvarchar(max) NOT NULL,
    [Precio] decimal(18,2) NOT NULL,
    [UsuarioEstablecimientoId] int NOT NULL,
    CONSTRAINT [PK_Canchas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Canchas_Usuarios_UsuarioEstablecimientoId] FOREIGN KEY ([UsuarioEstablecimientoId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Partidos] (
    [Id] int NOT NULL IDENTITY,
    [Ubicacion] nvarchar(max) NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [OrganizadorId] int NOT NULL,
    [UsuarioId] int NULL,
    CONSTRAINT [PK_Partidos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Partidos_Usuarios_OrganizadorId] FOREIGN KEY ([OrganizadorId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Partidos_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id])
);
GO

CREATE TABLE [Reservas] (
    [Id] int NOT NULL IDENTITY,
    [CanchaId] int NOT NULL,
    [FechaHora] datetime2 NOT NULL,
    [DuracionMinutos] int NOT NULL,
    [ClienteNombre] nvarchar(max) NOT NULL,
    [ClienteTelefono] nvarchar(max) NOT NULL,
    [ClienteEmail] nvarchar(max) NULL,
    [EsFrecuente] bit NOT NULL,
    [EstadoPago] nvarchar(max) NOT NULL,
    [Observaciones] nvarchar(max) NULL,
    [UsuarioEstablecimientoId] int NOT NULL,
    CONSTRAINT [PK_Reservas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reservas_Canchas_CanchaId] FOREIGN KEY ([CanchaId]) REFERENCES [Canchas] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reservas_Usuarios_UsuarioEstablecimientoId] FOREIGN KEY ([UsuarioEstablecimientoId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Calificaciones] (
    [Id] int NOT NULL IDENTITY,
    [PartidoId] int NOT NULL,
    [EvaluadorId] int NOT NULL,
    [EvaluadoId] int NOT NULL,
    [Puntaje] int NOT NULL,
    [Comentario] nvarchar(max) NOT NULL,
    [Fecha] datetime2 NOT NULL,
    CONSTRAINT [PK_Calificaciones] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Calificaciones_Partidos_PartidoId] FOREIGN KEY ([PartidoId]) REFERENCES [Partidos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Calificaciones_Usuarios_EvaluadoId] FOREIGN KEY ([EvaluadoId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Calificaciones_Usuarios_EvaluadorId] FOREIGN KEY ([EvaluadorId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Mensajes] (
    [Id] int NOT NULL IDENTITY,
    [PartidoId] int NOT NULL,
    [UsuarioId] int NOT NULL,
    [Contenido] nvarchar(max) NOT NULL,
    [Fecha] datetime2 NOT NULL,
    CONSTRAINT [PK_Mensajes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Mensajes_Partidos_PartidoId] FOREIGN KEY ([PartidoId]) REFERENCES [Partidos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Mensajes_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PartidoUsuario] (
    [PartidosId] int NOT NULL,
    [JugadoresId] int NOT NULL,
    CONSTRAINT [PK_PartidoUsuario] PRIMARY KEY ([PartidosId], [JugadoresId]),
    CONSTRAINT [FK_PartidoUsuario_Partidos_PartidosId] FOREIGN KEY ([PartidosId]) REFERENCES [Partidos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PartidoUsuario_Usuarios_JugadoresId] FOREIGN KEY ([JugadoresId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Rendimientos] (
    [Id] int NOT NULL IDENTITY,
    [PartidoId] int NOT NULL,
    [EvaluadorId] int NOT NULL,
    [EvaluadoId] int NOT NULL,
    [Actitud] int NOT NULL,
    [Pase] int NOT NULL,
    [Defensa] int NOT NULL,
    [TrabajoEquipo] int NOT NULL,
    [Puntualidad] int NOT NULL,
    [Fecha] datetime2 NOT NULL,
    CONSTRAINT [PK_Rendimientos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Rendimientos_Partidos_PartidoId] FOREIGN KEY ([PartidoId]) REFERENCES [Partidos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Rendimientos_Usuarios_EvaluadoId] FOREIGN KEY ([EvaluadoId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Rendimientos_Usuarios_EvaluadorId] FOREIGN KEY ([EvaluadorId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ReservaUsuarios] (
    [ReservaId] int NOT NULL,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_ReservaUsuarios] PRIMARY KEY ([ReservaId], [UsuarioId]),
    CONSTRAINT [FK_ReservaUsuarios_Reservas_ReservaId] FOREIGN KEY ([ReservaId]) REFERENCES [Reservas] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ReservaUsuarios_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Calificaciones_EvaluadoId] ON [Calificaciones] ([EvaluadoId]);
GO

CREATE INDEX [IX_Calificaciones_EvaluadorId] ON [Calificaciones] ([EvaluadorId]);
GO

CREATE INDEX [IX_Calificaciones_PartidoId] ON [Calificaciones] ([PartidoId]);
GO

CREATE INDEX [IX_Canchas_UsuarioEstablecimientoId] ON [Canchas] ([UsuarioEstablecimientoId]);
GO

CREATE INDEX [IX_Mensajes_PartidoId] ON [Mensajes] ([PartidoId]);
GO

CREATE INDEX [IX_Mensajes_UsuarioId] ON [Mensajes] ([UsuarioId]);
GO

CREATE INDEX [IX_Partidos_OrganizadorId] ON [Partidos] ([OrganizadorId]);
GO

CREATE INDEX [IX_Partidos_UsuarioId] ON [Partidos] ([UsuarioId]);
GO

CREATE INDEX [IX_PartidoUsuario_JugadoresId] ON [PartidoUsuario] ([JugadoresId]);
GO

CREATE INDEX [IX_Rendimientos_EvaluadoId] ON [Rendimientos] ([EvaluadoId]);
GO

CREATE INDEX [IX_Rendimientos_EvaluadorId] ON [Rendimientos] ([EvaluadorId]);
GO

CREATE INDEX [IX_Rendimientos_PartidoId] ON [Rendimientos] ([PartidoId]);
GO

CREATE INDEX [IX_Reservas_CanchaId] ON [Reservas] ([CanchaId]);
GO

CREATE INDEX [IX_Reservas_UsuarioEstablecimientoId] ON [Reservas] ([UsuarioEstablecimientoId]);
GO

CREATE INDEX [IX_ReservaUsuarios_UsuarioId] ON [ReservaUsuarios] ([UsuarioId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251003050218_Initial', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Reservas] ADD [FechaPago] datetime2 NULL;
GO

ALTER TABLE [Reservas] ADD [MetodoPago] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Reservas] ADD [SedeConfirmoTransferencia] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251027194747_AgregarCamposPagoReserva', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [DatosTarjetas] (
    [Id] int NOT NULL IDENTITY,
    [ReservaId] int NOT NULL,
    [HashToken] nvarchar(128) NOT NULL,
    [HashNumero] nvarchar(128) NOT NULL,
    [Ultimos4] nvarchar(4) NULL,
    [HashCvv] nvarchar(128) NULL,
    [NombreTitular] nvarchar(100) NULL,
    [FechaExpiracion] nvarchar(7) NULL,
    [FechaRegistroUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_DatosTarjetas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DatosTarjetas_Reservas_ReservaId] FOREIGN KEY ([ReservaId]) REFERENCES [Reservas] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_DatosTarjetas_ReservaId] ON [DatosTarjetas] ([ReservaId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251031180919_AddDatosTarjeta', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Mensajes] ADD [ReservaId] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251110230954_MensajesPorReserva', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Usuarios] ADD [EmailConfirmToken] nvarchar(max) NULL;
GO

ALTER TABLE [Usuarios] ADD [EmailConfirmTokenExpira] datetime2 NULL;
GO

ALTER TABLE [Usuarios] ADD [EmailConfirmado] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251118012323_AddEmailConfirmationToUsuario', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Usuarios] ADD [ResetPasswordToken] nvarchar(max) NULL;
GO

ALTER TABLE [Usuarios] ADD [ResetPasswordTokenExpira] datetime2 NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251118031407_AddResetPasswordFields', N'6.0.29');
GO

COMMIT;
GO

