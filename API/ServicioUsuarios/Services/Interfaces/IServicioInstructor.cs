using ServicioUsuarios.Data.DTOs;
using ServicioUsuarios.Data.DTOs.Instructor;

namespace ServicioUsuarios.Services.Interfaces;

public interface IServicioInstructor
{
    Task ValidarDatosRegistroAsync(RegistrarInstructorDTO instructorDTO);
    Task RegistrarAsync(RegistrarInstructorDTO instructorDto);
    Task<InstructorDTO> ActualizarAsync(HttpContext context, int idInstructor, ActualizarInstructorDTO docenteDto);
    Task EliminarAsync(HttpContext context, int idInstructor);
    Task<InstructorDTO?> ObtenerInstructorPorIdAsync(int idInstructor);
    Task CambiarContraseniaAsync(CambiarContraseniaDTO cambiarContraseniaDto, int idInstructor, HttpContext context);
}