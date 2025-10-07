namespace ServicioUsuarios.Data.DTOs.Instructor;
public class InstructorDTO
{
    public int idInstructor { get; set; } = 0;
    public string NombreCompleto { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public int IdGradoProfesional { get; set; } = 0;
}