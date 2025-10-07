using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ServicioUsuarios.Models;

namespace ServicioUsuarios.Data;

public partial class UsuariosDbContext : DbContext
{
    public UsuariosDbContext(DbContextOptions<UsuariosDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumno> Alumno { get; set; }

    public virtual DbSet<GradoEstudios> GradoEstudios { get; set; }

    public virtual DbSet<GradoProfesional> GradoProfesional { get; set; }

    public virtual DbSet<Instructor> Instructor { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasKey(e => e.IdAlumno).HasName("PRIMARY");

            entity.HasOne(d => d.IdGradoEstudiosNavigation).WithMany(p => p.Alumno).HasConstraintName("alumno-grado");
        });

        modelBuilder.Entity<GradoEstudios>(entity =>
        {
            entity.HasKey(e => e.IdGradoEstudios).HasName("PRIMARY");
        });

        modelBuilder.Entity<GradoProfesional>(entity =>
        {
            entity.HasKey(e => e.IdGradoProfesional).HasName("PRIMARY");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.IdInstructor).HasName("PRIMARY");

            entity.HasOne(d => d.IdGradoProfesionalNavigation).WithMany(p => p.Instructor).HasConstraintName("instructor-grado");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
