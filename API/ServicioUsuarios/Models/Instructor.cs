using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicioUsuarios.Models;

[Table("instructor")]
[Index("Correo", Name = "correo_UNIQUE", IsUnique = true)]
[Index("IdGradoProfesional", Name = "docente-grado_idx")]
[Index("NombreUsuario", Name = "nombreUsuario_UNIQUE", IsUnique = true)]
public partial class Instructor
{
    [Key]
    [Column("idInstructor")]
    public int IdInstructor { get; set; }

    [Column("nombreCompleto")]
    [StringLength(135)]
    public string NombreCompleto { get; set; } = null!;

    [Column("nombreUsuario")]
    [StringLength(45)]
    public string NombreUsuario { get; set; } = null!;

    [Column("correo")]
    [StringLength(45)]
    public string Correo { get; set; } = null!;

    [Column("contrasenia")]
    [StringLength(64)]
    public string Contrasenia { get; set; } = null!;

    [Column("idGradoProfesional")]
    public int IdGradoProfesional { get; set; }

    [Column("idFotoPerfil")]
    public int? IdFotoPerfil { get; set; }

    [Column("calificacion")]
    public float? Calificacion { get; set; }

    [ForeignKey("IdGradoProfesional")]
    [InverseProperty("Instructor")]
    public virtual GradoProfesional IdGradoProfesionalNavigation { get; set; } = null!;
}
