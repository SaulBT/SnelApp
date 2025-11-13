using ServicioUsuarios.Models;
using ServicioUsuarios.Services.Interfaces;
using ServicioUsuarios.Data.DTOs.Alumno;
using ServicioUsuarios.Data.DAOs.Interfaces;
using ServicioUsuarios.Validations;
using ServicioUsuarios.Data.DTOs;

namespace ServicioUsuarios.Services.Implementation;

public class ServicioAlumno : IServicioAlumno
{
    private readonly IAlumnoDAO _alumnoDAO;
    private readonly ILogger<ServicioAlumno> _logger;
    private readonly AlumnoValidaciones _validaciones;

    public ServicioAlumno(IAlumnoDAO alumnoDAO, ILogger<ServicioAlumno> logger, AlumnoValidaciones validaciones)
    {
        _alumnoDAO = alumnoDAO;
        _logger = logger;
        _validaciones = validaciones;
    }

    public async Task ValidarDatosRegistroAsync(RegistrarAlumnoDTO alumnoDTO)
    {
        _logger.LogInformation("Validando datos de registro de Alumno");
        await _validaciones.VerificarRegistroDeAlumnoAsync(alumnoDTO);
        _logger.LogInformation("Datos de registro de Alumno validados correctamente");
    }

    public async Task RegistrarAsync(RegistrarAlumnoDTO registrarAlumnoDto)
    {
        _logger.LogInformation("Registrando a Alumno");

        var alumnoNuevo = new Alumno
        {
            NombreCompleto = registrarAlumnoDto.NombreCompleto,
            NombreUsuario = registrarAlumnoDto.NombreUsuario,
            Contrasenia = registrarAlumnoDto.Contrasenia,
            Correo = registrarAlumnoDto.CorreoElectronico,
            IdGradoEstudios = registrarAlumnoDto.IdGradoEstudios
        };
        await _alumnoDAO.AgregarAlumnoAsync(alumnoNuevo);

        _logger.LogInformation($"Alumno registrado con éxito");
    }

    public async Task<AlumnoDTO> ActualizarAsync(HttpContext httpContext, int idAlumno, ActualizarAlumnoDTO actualizarAlumnoDto)
    {
        _logger.LogInformation("Actualizando a alumno");
        var alumno = await _validaciones.VerificarActualizacionDeAlumnoAsync(httpContext, idAlumno, actualizarAlumnoDto);

        alumno.NombreCompleto = actualizarAlumnoDto.NombreCompleto;
        alumno.NombreUsuario = actualizarAlumnoDto.NombreUsuario;
        alumno.IdGradoEstudios = actualizarAlumnoDto.IdGradoEstudios;
        alumno.IdFotoPerfil = actualizarAlumnoDto.IdFotoPerfil;
        await _alumnoDAO.ActualizarAsync(alumno);
        var retornoAlumno = new AlumnoDTO
        {
            IdAlumno = alumno.IdAlumno,
            NombreCompleto = actualizarAlumnoDto.NombreCompleto,
            NombreUsuario = actualizarAlumnoDto.NombreUsuario,
            CorreoElectronico = alumno.Correo,
            IdGradoEstudios = actualizarAlumnoDto.IdGradoEstudios,
            IdFotoPerfil = actualizarAlumnoDto.IdFotoPerfil
        };
        _logger.LogInformation($"Alumno actualizado con la id {retornoAlumno.IdAlumno}");
        return retornoAlumno;
    }

    public async Task EliminarAsync(HttpContext httpContext, int idAlumno)
    {
        _logger.LogInformation("Eliminando a Alumno");
        var alumno = await _validaciones.VerificarEliminacionDeAlumnoAsync(httpContext, idAlumno);

        await _alumnoDAO.EliminarAsync(alumno);
        _logger.LogInformation($"Alumno eliminado con la id {alumno.IdAlumno}");
    }

    public async Task<AlumnoDTO?> ObtenerAlumnoPorIdAsync(int idAlumno)
    {
        _logger.LogInformation("Buscando a un Alumno");
        var alumnoObtenido = await _validaciones.VerificarObtencionDeAlumnoAsync(idAlumno);

        var retornoAlumno = new AlumnoDTO
        {
            IdAlumno = alumnoObtenido.IdAlumno,
            NombreCompleto = alumnoObtenido.NombreCompleto,
            NombreUsuario = alumnoObtenido.NombreUsuario,
            CorreoElectronico = alumnoObtenido.Correo,
            IdGradoEstudios = (int)alumnoObtenido.IdGradoEstudios,
            IdFotoPerfil = alumnoObtenido.IdFotoPerfil
        };

        _logger.LogInformation("Alumno encontrado");
        return retornoAlumno;
    }

    public async Task CambiarContraseniaAsync(CambiarContraseniaDTO cambiarContraseniaDto, int idAlumno, HttpContext httpContext)
    {
        _logger.LogInformation("Cambiando contraseña de Alumno");
        var alumno = await _validaciones.VerificarCambioContraseniaAsync(cambiarContraseniaDto, idAlumno, httpContext);

        alumno.Contrasenia = cambiarContraseniaDto.ContraseniaNueva;
        await _alumnoDAO.ActualizarAsync(alumno);
        _logger.LogInformation($"Contraseña cambiada para el Alumno con la id {alumno.IdAlumno}");
    }
}