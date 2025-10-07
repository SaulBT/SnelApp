using ServicioUsuarios.Data.DAOs.Interfaces;
using ServicioUsuarios.Data.DTOs;
using ServicioUsuarios.Data.DTOs.Instructor;
using ServicioUsuarios.Exceptions;
using ServicioUsuarios.Models;

namespace ServicioUsuarios.Validations;

public class InstructorValidaciones
{
    private readonly IInstructorDAO _instructorDAO;

    public InstructorValidaciones(IInstructorDAO instructorDAO)
    {
        _instructorDAO = instructorDAO;
    }

    public async Task VerificarRegistroInstructorAsync(RegistrarInstructorDTO registrarInstructorDto)
    {
        verificarParametrosInstructorRegistro(registrarInstructorDto);
        await verificarInstructorNombreRegistroAsync(registrarInstructorDto.NombreUsuario);
        await verificarInstructorCorreoAsync(registrarInstructorDto.CorreoElectronico);
    }

    public async Task<Instructor> VerificarActualizacionDeInstructorAsync(HttpContext httpContext, int idInstructor, ActualizarInstructorDTO instructorDto)
    {
        verificarAutorizacion(httpContext);
        var idInstructorContexto = int.Parse(httpContext.User.FindFirst("idUsuario")!.Value);
        verificarIgualdadId(idInstructor, idInstructorContexto);
        verificarParametrosInstructorActualizacion(instructorDto);
        await verificarInstructorNombreActualizacionAsync(instructorDto.NombreUsuario, idInstructor);

        return await verificarExistenciaInstructorAsync(idInstructor);
    }

    public async Task<Instructor> VerificarEliminacionDeInstructorAsync(HttpContext httpContext, int idInstructor)
    {
        verificarAutorizacion(httpContext);
        var idInstructorContexto = int.Parse(httpContext.User.FindFirst("idUsuario")!.Value);
        verificarIdValida(idInstructor);
        verificarIdValida(idInstructorContexto);
        verificarIgualdadId(idInstructor, idInstructorContexto);
        return await verificarExistenciaInstructorAsync(idInstructor);
    }

    public async Task<Instructor> VerificarObtencionDeInstructorAsync(int idInstructor)
    {
        verificarIdValida(idInstructor);
        return await verificarExistenciaInstructorAsync(idInstructor);
    }

    public async Task<Instructor> VerificarCambioDeContraseniaAsync(CambiarContraseniaDTO cambiarContraseniaDTO, int idInstructor, HttpContext httpContext)
    {
        verificarParametrosCambiarContrasenia(cambiarContraseniaDTO);
        verificarAutorizacion(httpContext);
        var idInstructorContexto = int.Parse(httpContext.User.FindFirst("idUsuario")!.Value);
        verificarIdValida(idInstructorContexto);
        verificarIdValida(idInstructor);
        verificarIgualdadId(idInstructor, idInstructorContexto);
        var instructor = await verificarExistenciaInstructorAsync(idInstructor);
        verificarContraseniaActual(instructor, cambiarContraseniaDTO.ContraseniaActual);

        return instructor;
    }
    
    private void verificarIdValida(int id)
    {
        if (id <= 0)
        {
            throw new IdInvalidaException($"El id {id} es inválido.");
        }
    }

    private void verificarParametrosInstructorRegistro(RegistrarInstructorDTO instructorDto)
    {
        if (string.IsNullOrEmpty(instructorDto.NombreCompleto))
        {
            throw new CampoObligatorioException("El nombre completo es nulo");
        }
        else if (string.IsNullOrEmpty(instructorDto.NombreUsuario))
        {
            throw new CampoObligatorioException("El nombre usuario es nulo");
        }
        else if (string.IsNullOrEmpty(instructorDto.Contrasenia))
        {
            throw new CampoObligatorioException("La contraseña es nula");
        }
        else if (string.IsNullOrEmpty(instructorDto.CorreoElectronico))
        {
            throw new CampoObligatorioException("El correo electrónico es nulo");
        }
        else if (instructorDto.IdGradoProfesional <= 0)
        {
            throw new IdInvalidaException($"La id {instructorDto.IdGradoProfesional} del grado profesional es inválida.");
        }
    }

    private void verificarParametrosInstructorActualizacion(ActualizarInstructorDTO instructorDto)
    {
        if (string.IsNullOrEmpty(instructorDto.NombreUsuario))
        {
            throw new CampoObligatorioException("El nombre usuario es nulo");
        }
        else if (string.IsNullOrEmpty(instructorDto.NombreCompleto))
        {
            throw new CampoObligatorioException("El nombre completo es nulo");
        } else if (instructorDto.IdGradoProfesional <= 0)
        {
            throw new IdInvalidaException($"La id {instructorDto.IdGradoProfesional} del grado profesional es inválida.");
        }
    }

    private void verificarParametrosCambiarContrasenia(CambiarContraseniaDTO cambiarContraseniaDto)
    {
        if (string.IsNullOrEmpty(cambiarContraseniaDto.ContraseniaActual))
        {
            throw new CampoObligatorioException("La contraseña actual es nula.");
        } else if (string.IsNullOrEmpty(cambiarContraseniaDto.ContraseniaNueva))
        {
            throw new CampoObligatorioException("La nueva contraseña es nula.");
        }
    }

    private async Task<Instructor> verificarExistenciaInstructorAsync(int idInstructor)
    {
        var instructor = await _instructorDAO.ObtenerInstructorPorIdAsync(idInstructor);
        if (instructor == null)
        {
            throw new RecursoNoEncontradoException("El instructor no existe.");
        }

        return instructor;
    }

    private async Task verificarInstructorNombreRegistroAsync(string nombreUsuario)
    {
        var instructorExistente = await _instructorDAO.ObtenerInstructorPorNombreUsuarioAsync(nombreUsuario);
        if (instructorExistente != null)
        {
            throw new RecursoYaExistenteException($"'{nombreUsuario}' ya está en uso.");
        }
    }

    private async Task verificarInstructorNombreActualizacionAsync(string nombreUsuario, int id)
    {
        var instructorExistente = await _instructorDAO.ObtenerInstructorPorNombreUsuarioEIdAsync(nombreUsuario, id);
        if (instructorExistente != null)
        {
            throw new RecursoYaExistenteException($"'{nombreUsuario}' ya está en uso.");
        }
    }

    private void verificarIgualdadId(int id, int idInstructor)
    {
        if (id != idInstructor)
        {
            throw new DiscordanciaDeIdException("El Id del instructor no coincide con el ID proporcionado.");
        }
    }

    private async Task verificarInstructorCorreoAsync(string correo)
    {
        var instructorExistente = await _instructorDAO.ObtenerInstructorPorCorreoAsync(correo);
        if (instructorExistente != null)
        {
            throw new RecursoYaExistenteException($"'{correo}' ya está en uso.");
        }
    }

    private void verificarAutorizacion(HttpContext context)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAccessException("El usuario no está autenticado.");
        }
    }

    private void verificarContraseniaActual(Instructor instructor, string contraseniaActual)
    {
        if (instructor.Contrasenia != contraseniaActual)
        {
            throw new ContraseniaDiferenteException("La contraseña actual es incorrecta.");
        }
    }
}