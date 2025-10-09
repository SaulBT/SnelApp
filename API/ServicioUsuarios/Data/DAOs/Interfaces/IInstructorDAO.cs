using ServicioUsuarios.Models;

namespace ServicioUsuarios.Data.DAOs.Interfaces;

public interface IInstructorDAO
{
    Task ActualizarAsync(Instructor instructor);
    Task EliminarAsync(Instructor instructor);
    Task<Instructor?> ObtenerInstructorPorIdAsync(int id);
    Task<Instructor?> ObtenerInstructorPorNombreUsuarioAsync(string nombreUsuario);
    Task<Instructor?> ObtenerInstructorPorNombreUsuarioEIdAsync(string nombreUsuario, int id);
    Task<Instructor?> ObtenerInstructorPorCorreoAsync(string correo);
    Task AgregarInstructorAsync(Instructor docente);
    Task<Instructor> ObtenerInstructorPorNombreUsuarioOCorreoAsync(string nombreUsuarioOCorreo);
}