using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FutbolYa.WebAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<Rendimientos> Rendimientos { get; set; }
        public DbSet<Cancha> Canchas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<ReservaUsuario> ReservaUsuarios { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Partido>()
                .HasOne(p => p.Organizador)
                .WithMany()
                .HasForeignKey(p => p.OrganizadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rendimientos>()
                .HasOne(r => r.Evaluador)
                .WithMany()
                .HasForeignKey(r => r.EvaluadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rendimientos>()
                .HasOne(r => r.Evaluado)
                .WithMany()
                .HasForeignKey(r => r.EvaluadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.UsuarioEstablecimiento)
                .WithMany()
                .HasForeignKey(r => r.UsuarioEstablecimientoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservaUsuario>()
                .HasKey(ru => new { ru.ReservaId, ru.UsuarioId });

            modelBuilder.Entity<ReservaUsuario>()
                .HasOne(ru => ru.Reserva)
                .WithMany(r => r.Jugadores)
                .HasForeignKey(ru => ru.ReservaId);

            modelBuilder.Entity<ReservaUsuario>()
                .HasOne(ru => ru.Usuario)
                .WithMany()
                .HasForeignKey(ru => ru.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
