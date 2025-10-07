namespace ServicioUsuarios.Data.DTOs.Instructor;
public class ActualizarInstructorDTO
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public int IdGradoProfesional { get; set; } = 0;
}