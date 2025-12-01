using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FutbolYa.WebAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Partido> Partidos { get; set; }   // Puedes borrar si ya no usas Partido
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<Cancha> Canchas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<ReservaUsuario> ReservaUsuarios { get; set; }
        public DbSet<PartidoUsuario> PartidoUsuarios { get; set; }
        public DbSet<DatosTarjeta> DatosTarjetas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //
            // RELACIONES DE PARTIDO (si está en uso)
            //
            modelBuilder.Entity<Partido>()
                .HasOne(p => p.Organizador)
                .WithMany()
                .HasForeignKey(p => p.OrganizadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PartidoUsuario>()
                .ToTable("PartidoUsuario");

            modelBuilder.Entity<PartidoUsuario>()
                .HasKey(pu => new { pu.PartidosId, pu.JugadoresId });

            modelBuilder.Entity<PartidoUsuario>()
                .HasOne(pu => pu.Partido)
                .WithMany(p => p.Jugadores)
                .HasForeignKey(pu => pu.PartidosId);

            modelBuilder.Entity<PartidoUsuario>()
                .HasOne(pu => pu.Jugador)
                .WithMany()
                .HasForeignKey(pu => pu.JugadoresId);


            //
            // RELACIONES DE CALIFICACIONES (CORREGIDAS)
            //
            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Evaluador)
                .WithMany()
                .HasForeignKey(c => c.EvaluadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Evaluado)
                .WithMany()
                .HasForeignKey(c => c.EvaluadoId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔥 NUEVA RELACIÓN: CALIFICACIÓN → RESERVA
            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Reserva)
                .WithMany(r => r.Calificaciones)
                .HasForeignKey(c => c.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);

            //
            // RELACIONES DE RESERVAS
            //
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.UsuarioEstablecimiento)
                .WithMany()
                .HasForeignKey(r => r.UsuarioEstablecimientoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservaUsuario>()
                .HasKey(ru => new { ru.ReservaId, ru.UsuarioId });

            modelBuilder.Entity<ReservaUsuario>()
                .HasOne(ru => ru.Reserva)
                .WithMany(r => r.Jugadores)   // 🔥 ESTE ES EL NOMBRE CORRECTO
                .HasForeignKey(ru => ru.ReservaId);

            modelBuilder.Entity<ReservaUsuario>()
                .HasOne(ru => ru.Usuario)
                .WithMany()
                .HasForeignKey(ru => ru.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatosTarjeta>()
                .HasOne(dt => dt.Reserva)
                .WithMany(r => r.DatosTarjetas)
                .HasForeignKey(dt => dt.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<PartidoUsuario>()
                .ToTable("PartidoUsuario"); 

            modelBuilder.Entity<PartidoUsuario>()
                .HasKey(pu => new { pu.PartidosId, pu.JugadoresId });

            modelBuilder.Entity<PartidoUsuario>()
                .HasOne(pu => pu.Partido)
                .WithMany(p => p.Jugadores)
                .HasForeignKey(pu => pu.PartidosId);

            modelBuilder.Entity<PartidoUsuario>()
                .HasOne(pu => pu.Jugador)
                .WithMany()
                .HasForeignKey(pu => pu.JugadoresId);


            // 🔹 Relación opcional Mensaje–Reserva
            modelBuilder.Entity<Mensaje>()
                .HasOne(m => m.Reserva)
                .WithMany(r => r.Mensajes)        // o .WithMany() si no tenés la colección
                .HasForeignKey(m => m.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
