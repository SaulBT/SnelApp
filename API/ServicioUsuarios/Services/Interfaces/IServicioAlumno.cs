using ServicioUsuarios.Data.DTOs;
using ServicioUsuarios.Data.DTOs.Alumno;

namespace ServicioUsuarios.Services.Interfaces;

public interface IServicioAlumno
{
    Task ValidarDatosRegistroAsync(RegistrarAlumnoDTO alumnoDTO);
    Task RegistrarAsync(RegistrarAlumnoDTO alumnoDto);
    Task<AlumnoDTO> ActualizarAsync(HttpContext context, int idAlumno, ActualizarAlumnoDTO alumnoDto);
    Task EliminarAsync(HttpContext context, int idAlumno);
    Task<AlumnoDTO?> ObtenerAlumnoPorIdAsync(int idAlumno);
    Task CambiarContraseniaAsync(CambiarContraseniaDTO cambiarContraseniaDto, int idAlumno, HttpContext context);
}