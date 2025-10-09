using ServicioUsuarios.Data.DAOs.Interfaces;
using ServicioUsuarios.Data.DTOs;
using ServicioUsuarios.Data.DTOs.Alumno;
using ServicioUsuarios.Exceptions;
using ServicioUsuarios.Models;

namespace ServicioUsuarios.Validations;

public class LoginValidaciones
{
    private readonly IAlumnoDAO _alumnoDAO;
    private readonly IInstructorDAO _docenteDAO;

    public LoginValidaciones(IAlumnoDAO alumnoDAO, IInstructorDAO docenteDAO)
    {
        _alumnoDAO = alumnoDAO;
        _docenteDAO = docenteDAO;
    }

    public void VerificarParametrosUsuarioDto(IniciarSesionDTO iniciarSesionDto)
    {
        if (string.IsNullOrEmpty(iniciarSesionDto.TipoUsuario) ||
            string.IsNullOrEmpty(iniciarSesionDto.NombreUsuarioOCorreo) ||
            string.IsNullOrEmpty(iniciarSesionDto.Contrasenia))
        {
            throw new ArgumentNullException("Los parámetros del usuario no pueden ser nulos.");
        }

        if (string.IsNullOrEmpty(iniciarSesionDto.TipoUsuario))
        {
            throw new CampoObligatorioException("Los parámetros del usuario no pueden ser nulos: Tipo usuarios es nulo");
        }
        else if (iniciarSesionDto.TipoUsuario != "alumno" && iniciarSesionDto.TipoUsuario != "docente")
        {
            throw new TipoUsuarioInvalidoException($"Los parámetros del usuario son inválidos: El tipo de usuario es inválido: {iniciarSesionDto.TipoUsuario}");
        }
        else if (string.IsNullOrEmpty(iniciarSesionDto.NombreUsuarioOCorreo))
        {
            throw new CampoObligatorioException("Los parámetros del usuario no pueden ser nulos: Nombre de usuario o correo es nulo");
        }
        else if (string.IsNullOrEmpty(iniciarSesionDto.Contrasenia))
        {
            throw new CampoObligatorioException("Los parámetros del usuario no pueden ser nulos: La constraseña es nula");
        }
    }

    public async Task<Alumno> verificarCredencialesAlumnoAsync(IniciarSesionDTO usuarioDto)
    {
        var alumno = await _alumnoDAO.ObtenerPorNombreUsuarioOCorreoAsync(usuarioDto.NombreUsuarioOCorreo);
        if (alumno == null)
        {
            throw new UnauthorizedAccessException("No se encontró al alumno.");
        }
        else if (alumno.Contrasenia != usuarioDto.Contrasenia)
        {
            throw new UnauthorizedAccessException("Contraseña incorrecta");
        }

        return alumno;
    }

    public async Task<Instructor> verificarCredencialesInstructorAsync(IniciarSesionDTO usuarioDto)
    {
        var docente = await _docenteDAO.ObtenerInstructorPorNombreUsuarioOCorreoAsync(usuarioDto.NombreUsuarioOCorreo);
        if (docente == null)
        {
            throw new UnauthorizedAccessException("No se encontró al instructor.");
        }
        else if (docente.Contrasenia != usuarioDto.Contrasenia)
        {
            throw new UnauthorizedAccessException("Contraseña incorrecta");
        }

        return docente;
    }
}