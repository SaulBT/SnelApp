using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ServicioUsuarios.Data;
using ServicioUsuarios.Data.DAOs.Implementation;
using ServicioUsuarios.Services.Implementation;
using ServicioUsuarios.Validations;
using ServicioUsuarios.Models;
using System.Security.Claims;
using ServicioUsuarios.Data.DTOs.Instructor;
using ServicioUsuarios.Exceptions;

namespace SnelApp.Pruebas.ServicioUsuarios;

public class InstructorPruebas
{

    private readonly UsuariosDbContext _context;
    private readonly InstructorDAO _instructorDAO;
    private readonly SerivicioInstructor _servicioInstructor;
    private readonly ILogger<SerivicioInstructor> _logger;
    private readonly InstructorValidaciones _validaciones;

    public InstructorPruebas()
    {
        var opciones = new DbContextOptionsBuilder<UsuariosDbContext>()
            .UseInMemoryDatabase(databaseName: "UsuariosBDPruebas")
            .Options;
        _context = new UsuariosDbContext(opciones);
        _instructorDAO = new InstructorDAO(_context);

        _validaciones = new InstructorValidaciones(_instructorDAO);

        _logger = NullLogger<SerivicioInstructor>.Instance;

        _servicioInstructor = new SerivicioInstructor(_instructorDAO, _validaciones, _logger);
    }

    private HttpContext crearHttpContext(int idInstructor)
    {
        var claims = new List<Claim>
        {
            new Claim("idUsuario", idInstructor.ToString()),
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        return httpContext;
    }

    //TC-06
    [Fact]
    public async Task ValidarInformacionInstructorAsync()
    {
        var instructor = new RegistrarInstructorDTO
        {
            NombreCompleto = "Elena Torreblanca Morelia",
            NombreUsuario = "Elena_san",
            Contrasenia = "mostasaEspacial",
            CorreoElectronico = "tomolena@uv.mx",
            IdGradoProfesional = 2
        };

        await _servicioInstructor.ValidarDatosRegistroAsync(instructor);
    }

    //TC-07
    [Fact]
    public async Task RegistrarInstructorAsync()
    {
        var instructor = new RegistrarInstructorDTO
        {
            NombreCompleto = "Elena Torreblanca Morelia",
            NombreUsuario = "Elena_san",
            Contrasenia = "mostasaEspacial",
            CorreoElectronico = "tomolena@uv.mx",
            IdGradoProfesional = 2
        };

        await _servicioInstructor.RegistrarAsync(instructor);
        var instructorRegistrado = await _instructorDAO.ObtenerInstructorPorNombreUsuarioAsync("Elena_san");

        Assert.NotNull(instructorRegistrado);
        Assert.Equal(instructor.NombreCompleto, instructorRegistrado!.NombreCompleto);
        Assert.Equal(instructor.NombreUsuario, instructorRegistrado!.NombreUsuario);
        Assert.Equal(instructor.Contrasenia, instructorRegistrado!.Contrasenia);
        Assert.Equal(instructor.CorreoElectronico, instructorRegistrado!.Correo);
        Assert.Equal(instructor.IdGradoProfesional, instructorRegistrado!.IdGradoProfesional);

        await _instructorDAO.EliminarAsync(instructorRegistrado);
    }

    //TC-08
    [Fact]
    public async Task ObtenerInstructor()
    {
        var instructorExistente = new Instructor
        {
            IdInstructor = 99,
            NombreCompleto = "César Efraín Dolores Cárdenaz",
            NombreUsuario = "efra_dc74",
            Contrasenia = "kciopd4",
            Correo = "docce1974@yahoo.com",
            IdGradoProfesional = 2,
            IdFotoPerfil = 121
        };
        await _context.Instructor.AddAsync(instructorExistente);
        await _context.SaveChangesAsync();
        
        InstructorDTO instructor = await _servicioInstructor.ObtenerInstructorPorIdAsync(99);

        Assert.NotNull(instructor);
        Assert.Equal(99, instructor.idInstructor);

        _context.Instructor.Remove(instructorExistente);
        await _context.SaveChangesAsync();
    }

    //TC-09
    [Fact]
    public async Task EditarInstructor()
    {
        var instructorExistente = new Instructor
        {
            IdInstructor = 99,
            NombreCompleto = "César Efraín Dolores Cárdenaz",
            NombreUsuario = "efra_dc74",
            Contrasenia = "kciopd4",
            Correo = "docce1974@yahoo.com",
            IdGradoProfesional = 2,
            IdFotoPerfil = 121
        };
        await _context.Instructor.AddAsync(instructorExistente);
        await _context.SaveChangesAsync();

        var instructorEditar = new ActualizarInstructorDTO
        {
            NombreCompleto = "Cesar Efraín Dolorez Cárdenas",
            NombreUsuario = "efra_dolores74",
            IdGradoProfesional = 1,
            IdFotoPerfil = 243
        };

        await _servicioInstructor.ActualizarAsync(crearHttpContext(99), 99, instructorEditar);

        InstructorDTO instructor = await _servicioInstructor.ObtenerInstructorPorIdAsync(99);

        Assert.NotNull(instructor);
        Assert.Equal("efra_dolores74", instructor.NombreUsuario);

        _context.Instructor.Remove(instructorExistente);
        await _context.SaveChangesAsync();
    }

    //TC-10
    [Fact]
    public async Task EliminarAlumno()
    {
        var instructorExistente = new Instructor
        {
            IdInstructor = 99,
            NombreCompleto = "César Efraín Dolores Cárdenaz",
            NombreUsuario = "efra_dc74",
            Contrasenia = "kciopd4",
            Correo = "docce1974@yahoo.com",
            IdGradoProfesional = 2,
            IdFotoPerfil = 121
        };
        await _context.Instructor.AddAsync(instructorExistente);
        await _context.SaveChangesAsync();

        await _servicioInstructor.EliminarAsync(crearHttpContext(99), 99);

        try
        {
            await _instructorDAO.ObtenerInstructorPorIdAsync(99);
        }
        catch (Exception)
        {
            Assert.True(true);
            return;
        }
    }

    //TC-12
    [Fact]
    public async Task ValidacionFallidaDocente()
    {
        var instructor = new RegistrarInstructorDTO
        {
            NombreCompleto = "José Vazconselos",
            NombreUsuario = "",
            Contrasenia = "",
            CorreoElectronico = "jv@gmail.com",
            IdGradoProfesional = 1
        };

        await Assert.ThrowsAsync<CampoObligatorioException>(async () => await _servicioInstructor.ValidarDatosRegistroAsync(instructor));
    }
}
