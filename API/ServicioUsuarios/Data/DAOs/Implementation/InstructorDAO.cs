using Microsoft.EntityFrameworkCore;
using ServicioUsuarios.Models;
using ServicioUsuarios.Data.DAOs.Interfaces;

namespace ServicioUsuarios.Data.DAOs.Implementation;

public class InstructorDAO : IInstructorDAO
{
    private readonly UsuariosDbContext _context;

    public InstructorDAO(UsuariosDbContext context)
    {
        _context = context;
    }

    public async Task ActualizarAsync(Instructor instructor)
    {
        try
        {
            var instructorExistente = await _context.Instructor.FindAsync(instructor.IdInstructor);
            if (instructorExistente != null)
            {
                instructorExistente.NombreCompleto = instructor.NombreCompleto;
                instructorExistente.NombreUsuario = instructor.NombreUsuario;
                instructorExistente.Correo = instructor.Correo;
                instructorExistente.IdGradoProfesional = instructor.IdGradoProfesional;

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el instructor: " + ex.Message + " - " + ex.InnerException?.Message);
        }
    }

    public async Task EliminarAsync(Instructor instructor)
    {
        try
        {
            _context.Instructor.Remove(instructor);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el instructor: " + ex.Message + " - " + ex.InnerException?.Message);
        }
    }

    public async Task<Instructor?> ObtenerInstructorPorIdAsync(int id)
    {
        try
        {
            return await _context.Instructor.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el instructor por ID: " + ex.Message + " - " + ex.InnerException?.Message);
        }
    }

    public async Task<Instructor?> ObtenerInstructorPorNombreUsuarioAsync(string nombreUsuario)
    {
        try
        {
            return await _context.Instructor
                .FirstOrDefaultAsync(d => d.NombreUsuario == nombreUsuario);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el instructor por nombre de usuario: " + ex.Message + " - " + ex.InnerException?.Message);
        }
    }

    public async Task<Instructor?> ObtenerInstructorPorNombreUsuarioEIdAsync(string nombreUsuario, int id)
    {
        try
        {
            return await _context.Instructor
                .FirstOrDefaultAsync(d => d.NombreUsuario == nombreUsuario && d.IdInstructor != id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el instructor por nombre de usuario y ID: " + ex.Message + " - " + ex.InnerException?.Message);
        }
    }

    public async Task<Instructor?> ObtenerInstructorPorCorreoAsync(string correo)
    {
        try
        {
            return await _context.Instructor
                .FirstOrDefaultAsync(d => d.Correo == correo);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el instructor por correo: " + ex.Message + " - " + ex.InnerException?.Message);
        }
    }

    public async Task AgregarInstructorAsync(Instructor instructor)
    {
        try
        {
            await _context.Instructor.AddAsync(instructor);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al registrar el instructor: " + ex.Message + " - " + ex.InnerException?.Message);
        }
    }

    public async Task<Instructor> ObtenerInstructorPorNombreUsuarioOCorreoAsync(string nombreUsuarioOCorreo)
    {
        try
        {
            return await _context.Instructor
                .Where(d => d.NombreUsuario == nombreUsuarioOCorreo || d.Correo == nombreUsuarioOCorreo)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el instructor por nombre de usuario o correo: " + ex.Message + " - " + ex.InnerException?.Message);
        }
    }
}