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
        var idInstructorContexto = int.Parse(httpContext.User.FindFirst("idUsuario")!.Value);
        verificarId(idInstructorContexto, "Token");
        verificarId(idInstructor, "Instructor");
        verificarIgualdadId(idInstructor, idInstructorContexto);
        verificarParametrosInstructorActualizacion(instructorDto);
        await verificarInstructorNombreActualizacionAsync(instructorDto.NombreUsuario, idInstructor);

        return await verificarExistenciaInstructorAsync(idInstructor);
    }

    public async Task<Instructor> VerificarEliminacionDeInstructorAsync(HttpContext httpContext, int idInstructor)
    {
        var idInstructorContexto = int.Parse(httpContext.User.FindFirst("idUsuario")!.Value);
        verificarId(idInstructorContexto, "Token");
        verificarId(idInstructor, "Instructor");
        verificarIgualdadId(idInstructor, idInstructorContexto);
        return await verificarExistenciaInstructorAsync(idInstructor);
    }

    public async Task<Instructor> VerificarObtencionDeInstructorAsync(int idInstructor)
    {
        verificarId(idInstructor, "Instructor");
        return await verificarExistenciaInstructorAsync(idInstructor);
    }

    public async Task<Instructor> VerificarCambioDeContraseniaAsync(CambiarContraseniaDTO cambiarContraseniaDTO, int idInstructor, HttpContext httpContext)
    {
        verificarParametrosCambiarContrasenia(cambiarContraseniaDTO);
        var idInstructorContexto = int.Parse(httpContext.User.FindFirst("idUsuario")!.Value);
        verificarId(idInstructorContexto, "Token");
        verificarId(idInstructor, "Instructor");
        verificarIgualdadId(idInstructor, idInstructorContexto);
        var instructor = await verificarExistenciaInstructorAsync(idInstructor);
        verificarContraseniaActual(instructor, cambiarContraseniaDTO.ContraseniaActual);

        return instructor;
    }
    
    private void verificarId(int id, string actor)
    {
        if (id <= 0)
        {
            throw new IdInvalidaException(id, actor);
        }
    }

    private void verificarParametrosInstructorRegistro(RegistrarInstructorDTO instructorDto)
    {
        if (string.IsNullOrEmpty(instructorDto.NombreCompleto))
        {
            throw new CampoObligatorioException("Nombre completo");
        }
        else if (string.IsNullOrEmpty(instructorDto.NombreUsuario))
        {
            throw new CampoObligatorioException("Nombre de usuario");
        }
        else if (string.IsNullOrEmpty(instructorDto.Contrasenia))
        {
            throw new CampoObligatorioException("Contraseña");
        }
        else if (string.IsNullOrEmpty(instructorDto.CorreoElectronico))
        {
            throw new CampoObligatorioException("Correo electrónico");
        }
        else if (instructorDto.IdGradoProfesional <= 0)
        {
            throw new IdInvalidaException(instructorDto.IdGradoProfesional, "Grado profesional");
        }
    }

    private void verificarParametrosInstructorActualizacion(ActualizarInstructorDTO instructorDto)
    {
        if (string.IsNullOrEmpty(instructorDto.NombreUsuario))
        {
            throw new CampoObligatorioException("Nombre de usuario");
        }
        else if (string.IsNullOrEmpty(instructorDto.NombreCompleto))
        {
            throw new CampoObligatorioException("Nombre completo");
        }
        else if (instructorDto.IdGradoProfesional <= 0)
        {
            throw new IdInvalidaException(instructorDto.IdGradoProfesional, "Grado profesional");
        }
    }

    private void verificarParametrosCambiarContrasenia(CambiarContraseniaDTO cambiarContraseniaDto)
    {
        if (string.IsNullOrEmpty(cambiarContraseniaDto.ContraseniaActual))
        {
            throw new CampoObligatorioException("Contraseña actual");
        } else if (string.IsNullOrEmpty(cambiarContraseniaDto.ContraseniaNueva))
        {
            throw new CampoObligatorioException("Contraseña nueva");
        }
    }

    private async Task<Instructor> verificarExistenciaInstructorAsync(int idInstructor)
    {
        var instructor = await _instructorDAO.ObtenerInstructorPorIdAsync(idInstructor);
        if (instructor == null)
        {
            throw new RecursoNoEncontradoException("Instructor");
        }

        return instructor;
    }

    private async Task verificarInstructorNombreRegistroAsync(string nombreUsuario)
    {
        var instructorExistente = await _instructorDAO.ObtenerInstructorPorNombreUsuarioAsync(nombreUsuario);
        if (instructorExistente != null)
        {
            throw new RecursoYaExistenteException(nombreUsuario);
        }
    }

    private async Task verificarInstructorNombreActualizacionAsync(string nombreUsuario, int id)
    {
        var instructorExistente = await _instructorDAO.ObtenerInstructorPorNombreUsuarioEIdAsync(nombreUsuario, id);
        if (instructorExistente != null)
        {
            throw new RecursoYaExistenteException(nombreUsuario);
        }
    }

    private void verificarIgualdadId(int id, int idInstructor)
    {
        if (id != idInstructor)
        {
            throw new DiscordanciaDeIdException(id, idInstructor, "Instructor");
        }
    }

    private async Task verificarInstructorCorreoAsync(string correo)
    {
        var instructorExistente = await _instructorDAO.ObtenerInstructorPorCorreoAsync(correo);
        if (instructorExistente != null)
        {
            throw new RecursoYaExistenteException(correo);
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