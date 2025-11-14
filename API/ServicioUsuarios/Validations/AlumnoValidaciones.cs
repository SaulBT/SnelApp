using ServicioUsuarios.Data.DAOs.Interfaces;
using ServicioUsuarios.Data.DTOs;
using ServicioUsuarios.Data.DTOs.Alumno;
using ServicioUsuarios.Models;
using ServicioUsuarios.Exceptions;

namespace ServicioUsuarios.Validations;

public class AlumnoValidaciones
{
    private readonly IAlumnoDAO _alumnoDAO;

    public AlumnoValidaciones(IAlumnoDAO alumnoDAO)
    {
        _alumnoDAO = alumnoDAO;
    }

    public async Task VerificarRegistroDeAlumnoAsync(RegistrarAlumnoDTO registrarAlumnoDto)
    {
        verificarParametrosAlumnoRegistro(registrarAlumnoDto);
        await verificarAlumnoNombreRegistroAsync(registrarAlumnoDto.NombreUsuario);
        await verificarAlumnoCorreoAsync(registrarAlumnoDto.CorreoElectronico);
    }

    public async Task<Alumno> VerificarActualizacionDeAlumnoAsync(HttpContext httpContext, int idAlumno, ActualizarAlumnoDTO alumnoDto)
    {
        var idAlumnoContext = int.Parse(httpContext.User.FindFirst("idUsuario")!.Value);
        verificarId(idAlumno);
        verificarIgualdadId(idAlumno, idAlumnoContext);
        verificarParametrosAlumnoActualizacion(alumnoDto);
        await verificarAlumnoNombreActualizacionAsync(alumnoDto.NombreUsuario, idAlumno);
        return await VerificarExistenciaAlumno(idAlumno);
    }

    public async Task<Alumno> VerificarEliminacionDeAlumnoAsync(HttpContext httpContext, int idAlumno)
    {
        var idAlumnoContext = int.Parse(httpContext.User.FindFirst("idUsuario")!.Value);
        verificarIgualdadId(idAlumnoContext, idAlumno);
        verificarId(idAlumno);

        return await VerificarExistenciaAlumno(idAlumno);
    }

    public async Task<Alumno> VerificarObtencionDeAlumnoAsync(int idAlumno)
    {
        verificarId(idAlumno);
        return await VerificarExistenciaAlumno(idAlumno);
    }

    public async Task<Alumno> VerificarCambioContraseniaAsync(CambiarContraseniaDTO cambiarContraseniaDto, int idAlumno, HttpContext httpContext)
    {
        verificarParametrosCambiarContrasenia(cambiarContraseniaDto);
        var idAlumnoContext = int.Parse(httpContext.User.FindFirst("idUsuario")!.Value);
        verificarIgualdadId(idAlumnoContext, idAlumno);
        verificarId(idAlumno);
        var alumno = await VerificarExistenciaAlumno(idAlumno);
        verificarContraseniaActual(alumno, cambiarContraseniaDto.ContraseniaActual);

        return alumno;
    }

    private void verificarId(int idUsuario)
    {
        if (idUsuario <= 0)
        {
            throw new IdInvalidaException(idUsuario, "Alumno");
        }
    }

    private void verificarParametrosAlumnoRegistro(RegistrarAlumnoDTO alumnoDto)
    {
        if (string.IsNullOrEmpty(alumnoDto.NombreCompleto))
        {
            throw new CampoObligatorioException("Nombre completo");
        }
        else if (string.IsNullOrEmpty(alumnoDto.NombreUsuario))
        {
            throw new CampoObligatorioException("Nombre de usuario");
        }
        else if (string.IsNullOrEmpty(alumnoDto.Contrasenia))
        {
            throw new CampoObligatorioException("Contraseña");
        }
        else if (string.IsNullOrEmpty(alumnoDto.CorreoElectronico))
        {
            throw new CampoObligatorioException("Correo electrónico");
        }
        else if (alumnoDto.IdGradoEstudios <= 0)
        {
            throw new IdInvalidaException(alumnoDto.IdGradoEstudios, "Grado de estudios");
        }
    }

    private void verificarParametrosAlumnoActualizacion(ActualizarAlumnoDTO alumnoDto)
    {
        if (string.IsNullOrEmpty(alumnoDto.NombreCompleto))
        {
            throw new CampoObligatorioException("Nombre completo");
        }
        else if (string.IsNullOrEmpty(alumnoDto.NombreUsuario))
        {
            throw new CampoObligatorioException("Nombre de usuario");
        }
        else if (alumnoDto.IdGradoEstudios <= 0)
        {
            throw new IdInvalidaException(alumnoDto.IdGradoEstudios, "Grado de estudios");
        }
    }

    private void verificarParametrosCambiarContrasenia(CambiarContraseniaDTO cambiarContraseniaDto)
    {
        if (string.IsNullOrEmpty(cambiarContraseniaDto.ContraseniaNueva))
        {
            throw new CampoObligatorioException("Contraseña nueva");
        }
        else if (string.IsNullOrEmpty(cambiarContraseniaDto.ContraseniaActual))
        {
            throw new CampoObligatorioException("Contraseña actual");
        }
    }

    public async Task<Alumno> VerificarExistenciaAlumno(int idAlumno)
    {
        var alumnoObtenido = await _alumnoDAO.ObtenerAlumnoPorIdAsync(idAlumno);
        if (alumnoObtenido == null)
        {
            throw new RecursoNoEncontradoException("Alumno");
        }
        return alumnoObtenido;
    }

    private async Task verificarAlumnoNombreRegistroAsync(string nombreUsuario)
    {
        var alumnoExistente = await _alumnoDAO.ObtenerPorNombreUsuarioAsync(nombreUsuario);
        if (alumnoExistente != null)
        {
            throw new RecursoYaExistenteException(nombreUsuario);
        }
    }

    private async Task verificarAlumnoNombreActualizacionAsync(string nombreUsuario, int id)
    {
        var alumnoExistente = await _alumnoDAO.ObtenerPorNombreUsuarioEIdAsync(nombreUsuario, id);
        if (alumnoExistente != null)
        {
            throw new RecursoYaExistenteException(nombreUsuario);
        }
    }

    private void verificarIgualdadId(int id, int idAlumno)
    {
        if (id != idAlumno)
        {
            throw new DiscordanciaDeIdException(idAlumno, id, "Alumno");
        }
    }

    private async Task verificarAlumnoCorreoAsync(string correo)
    {
        var alumnoExistente = await _alumnoDAO.ObtenerPorCorreoAsync(correo);
        if (alumnoExistente != null)
        {
            throw new RecursoYaExistenteException(correo);
        }
    }

    private void verificarContraseniaActual(Alumno alumno, string contraseniaActual)
    {
        if (alumno.Contrasenia != contraseniaActual)
        {
            throw new ContraseniaDiferenteException("La contraseña actual es incorrecta.");
        }
    }
}