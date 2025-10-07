using ServicioUsuarios.Data.DAOs.Interfaces;
using ServicioUsuarios.Data.DTOs;
using ServicioUsuarios.Models;
using ServicioUsuarios.Services.Interfaces;
using ServicioUsuarios.Data.DTOs.Instructor;
using ServicioUsuarios.Validations;

namespace ServicioUsuarios.Services.Implementation;

public class SerivicioInstructor : IServicioInstructor
{
    private readonly IInstructorDAO _instructorDAO;
    private readonly InstructorValidaciones _validaciones;
    private readonly ILogger<SerivicioInstructor> _logger;

    public SerivicioInstructor(IInstructorDAO instructorDAO, InstructorValidaciones validaciones, ILogger<SerivicioInstructor> logger)
    {
        _instructorDAO = instructorDAO;
        _validaciones = validaciones;
        _logger = logger;
    }

    public async Task RegistrarAsync(RegistrarInstructorDTO registrarInstructorDto)
    {
        _logger.LogInformation("Registrando a Instructor");
        await _validaciones.VerificarRegistroInstructorAsync(registrarInstructorDto);

        var nuevoInstructor = new Instructor
        {
            NombreCompleto = registrarInstructorDto.NombreCompleto,
            NombreUsuario = registrarInstructorDto.NombreUsuario,
            Contrasenia = registrarInstructorDto.Contrasenia,
            Correo = registrarInstructorDto.CorreoElectronico,
            IdGradoProfesional = registrarInstructorDto.IdGradoProfesional
        };
        await _instructorDAO.AgregarInstructorAsync(nuevoInstructor);

        _logger.LogInformation($"Se registró al Instructor corectamente");
    }

    public async Task<InstructorDTO> ActualizarAsync(HttpContext httpContext, int idInstructor, ActualizarInstructorDTO actualizarInstructorDto)
    {
        _logger.LogInformation("Se actualiza un Instructor");
        var instructor = await _validaciones.VerificarActualizacionDeInstructorAsync(httpContext, idInstructor, actualizarInstructorDto);

        instructor.NombreCompleto = actualizarInstructorDto.NombreCompleto;
        instructor.NombreUsuario = actualizarInstructorDto.NombreUsuario;
        instructor.IdGradoProfesional = actualizarInstructorDto.IdGradoProfesional;
        await _instructorDAO.ActualizarAsync(instructor);

        var retornoInstructor = new InstructorDTO
        {
            idInstructor = instructor.IdInstructor,
            NombreCompleto = instructor.NombreCompleto,
            NombreUsuario = instructor.NombreUsuario,
            CorreoElectronico = instructor.Correo,
            IdGradoProfesional = (int)instructor.IdGradoProfesional
        };
        _logger.LogInformation($"Se actualizó al Instructor con la id {retornoInstructor.idInstructor}");
        return retornoInstructor;
    }

    public async Task EliminarAsync(HttpContext httpContexto, int idInstructor)
    {
        _logger.LogInformation("Eliminando a un Instructor");
        var instructor = await _validaciones.VerificarEliminacionDeInstructorAsync(httpContexto, idInstructor);
        await _instructorDAO.EliminarAsync(instructor);
        _logger.LogInformation($"Se eliminó al Instructor con la id {instructor.IdInstructor}");
    }

    public async Task<InstructorDTO?> ObtenerInstructorPorIdAsync(int idInstructor)
    {
        _logger.LogInformation("Buscando a un Instructor");
        var instructor = await _validaciones.VerificarObtencionDeInstructorAsync(idInstructor);

        var retornoInstructor = new InstructorDTO
        {
            idInstructor = instructor.IdInstructor,
            NombreCompleto = instructor.NombreCompleto,
            NombreUsuario = instructor.NombreUsuario,
            CorreoElectronico = instructor.Correo,
            IdGradoProfesional = (int)instructor.IdGradoProfesional
        };
        _logger.LogInformation($"Instructor encontrado con la id: {retornoInstructor.idInstructor}");
        return retornoInstructor;
    }

    public async Task CambiarContraseniaAsync(CambiarContraseniaDTO cambiarContraseniaDto, int idInstructor, HttpContext httpContext)
    {
        _logger.LogInformation("Cambiando la contraseña de un Instructor");
        var instructor = await _validaciones.VerificarCambioDeContraseniaAsync(cambiarContraseniaDto, idInstructor, httpContext);

        instructor.Contrasenia = cambiarContraseniaDto.ContraseniaNueva;
        _logger.LogInformation($"Contraseña de un Instructor cambiada con la id {instructor.IdInstructor}");
        await _instructorDAO.ActualizarAsync(instructor);
    }
}