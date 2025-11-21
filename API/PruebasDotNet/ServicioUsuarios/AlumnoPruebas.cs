using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ServicioUsuarios.Data;
using ServicioUsuarios.Data.DAOs.Implementation;
using ServicioUsuarios.Data.DTOs.Alumno;
using ServicioUsuarios.Services.Implementation;
using ServicioUsuarios.Validations;
using ServicioUsuarios.Models;
using System.Security.Claims;
using ServicioUsuarios.Exceptions;
using ServicioUsuarios.Data.DTOs;

namespace SnelApp.Pruebas.ServicioUsuarios;

public class AlumnoPruebas
{

    private readonly UsuariosDbContext _context;
    private readonly AlumnoDAO _alumnoDAO;
    private readonly ServicioAlumno _servicioAlumno;
    private readonly ILogger<ServicioAlumno> _logger;
    private readonly AlumnoValidaciones _validaciones;

    public AlumnoPruebas()
    {
        var opciones = new DbContextOptionsBuilder<UsuariosDbContext>()
            .UseInMemoryDatabase(databaseName: "UsuariosBDPruebas")
            .Options;
        _context = new UsuariosDbContext(opciones);
        _alumnoDAO = new AlumnoDAO(_context);

        _validaciones = new AlumnoValidaciones(_alumnoDAO);

        _logger = NullLogger<ServicioAlumno>.Instance;

        _servicioAlumno = new ServicioAlumno(_alumnoDAO, _logger, _validaciones);
    }

    private HttpContext crearHttpContext(int idAlumno)
    {
        var claims = new List<Claim>
        {
            new Claim("idUsuario", idAlumno.ToString()),
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        return httpContext;
    }

    //TC-01
    [Fact]
    public async Task ValidarInformacionAlumnoAsync()
    {
        var alumno = new RegistrarAlumnoDTO
        {
            NombreCompleto = "Manuel Alexander Martínez Tejeda",
            NombreUsuario = "AlexanderMT",
            Contrasenia = "7yairi7",
            CorreoElectronico = "malexander@hotmail.com",
            IdGradoEstudios = 3
        };

        await _servicioAlumno.ValidarDatosRegistroAsync(alumno);
    }

    //TC-02
    [Fact]
    public async Task RegistrarAlumnoAsync()
    {
        var alumno = new RegistrarAlumnoDTO
        {
            NombreCompleto = "Manuel Alexander Martínez Tejeda",
            NombreUsuario = "AlexanderMT",
            Contrasenia = "7yairi7",
            CorreoElectronico = "malexander@hotmail.com",
            IdGradoEstudios = 3
        };

        await _servicioAlumno.RegistrarAsync(alumno);
        var alumnoRegistrado = await _alumnoDAO.ObtenerPorNombreUsuarioAsync("AlexanderMT");

        Assert.NotNull(alumnoRegistrado);
        Assert.Equal(alumno.NombreCompleto, alumnoRegistrado!.NombreCompleto);
        Assert.Equal(alumno.NombreUsuario, alumnoRegistrado!.NombreUsuario);
        Assert.Equal(alumno.Contrasenia, alumnoRegistrado!.Contrasenia);
        Assert.Equal(alumno.CorreoElectronico, alumnoRegistrado!.Correo);
        Assert.Equal(alumno.IdGradoEstudios, alumnoRegistrado!.IdGradoEstudios);

        await _alumnoDAO.EliminarAsync(alumnoRegistrado);
    }

    //TC-03
    [Fact]
    public async Task ObtenerAlumno()
    {
        var alumnoExistente = new Alumno
        {
            IdAlumno = 488,
            NombreCompleto = "Alejandra Evelyn López Escamilla",
            NombreUsuario = "AleEsca00",
            Contrasenia = "contra123",
            Correo = "correo@ejemplo.com",
            IdGradoEstudios = 4,
            IdFotoPerfil = 421
        };
        await _context.Alumno.AddAsync(alumnoExistente);
        await _context.SaveChangesAsync();
        
        AlumnoDTO alumno = await _servicioAlumno.ObtenerAlumnoPorIdAsync(488);

        Assert.NotNull(alumno);
        Assert.Equal(488, alumno.IdAlumno);

        _context.Alumno.Remove(alumnoExistente);
        await _context.SaveChangesAsync();
    }

    //TC-04
    [Fact]
    public async Task EditarAlumno()
    {
        var alumnoExistente = new Alumno
        {
            IdAlumno = 488,
            NombreCompleto = "Alejandra Evelyn López Escamilla",
            NombreUsuario = "AleEsca00",
            Contrasenia = "contra123",
            Correo = "correo@ejemplo.com",
            IdGradoEstudios = 4,
            IdFotoPerfil = 421
        };
        await _context.Alumno.AddAsync(alumnoExistente);
        await _context.SaveChangesAsync();

        var alumnoEditar = new ActualizarAlumnoDTO
        {
            NombreCompleto = "Alejandra Evelyn López Escamilla",
            NombreUsuario = "Alecita00",
            IdGradoEstudios = 4,
            IdFotoPerfil = 421
        };

        await _servicioAlumno.ActualizarAsync(crearHttpContext(488), 488, alumnoEditar);

        AlumnoDTO alumno = await _servicioAlumno.ObtenerAlumnoPorIdAsync(488);

        Assert.NotNull(alumno);
        Assert.Equal("Alecita00", alumno.NombreUsuario);

        _context.Alumno.Remove(alumnoExistente);
        await _context.SaveChangesAsync();
    }

    //TC-05
    [Fact]
    public async Task EliminarAlumno()
    {
        var alumnoExistente = new Alumno
        {
            IdAlumno = 488,
            NombreCompleto = "Alejandra Evelyn López Escamilla",
            NombreUsuario = "AleEsca00",
            Contrasenia = "contra123",
            Correo = "correo@ejemplo.com",
            IdGradoEstudios = 4,
            IdFotoPerfil = 421
        };
        await _context.Alumno.AddAsync(alumnoExistente);
        await _context.SaveChangesAsync();

        await _servicioAlumno.EliminarAsync(crearHttpContext(488), 488);

        try
        {
            await _alumnoDAO.ObtenerAlumnoPorIdAsync(488);
        }
        catch (Exception)
        {
            Assert.True(true);
            return;
        }
    }

    //TC-11
    [Fact]
    public async Task ValidacionFallidaAlumno()
    {
        var alumno = new RegistrarAlumnoDTO
        {
            NombreCompleto = "",
            NombreUsuario = "",
            Contrasenia = "",
            CorreoElectronico = "sabaton.com",
            IdGradoEstudios = 3
        };

        await Assert.ThrowsAsync<CampoObligatorioException>(async () => await _servicioAlumno.ValidarDatosRegistroAsync(alumno));
    }

    //TC-17
    [Fact]
    public async Task CambiarContraseniaAlumno()
    {
        var alumnoExistente = new Alumno
        {
            IdAlumno = 57,
            NombreCompleto = "Nombre Ejemplo",
            NombreUsuario = "UsuarioEjemplo",
            Contrasenia = "4anaka3",
            Correo = "correo@ejemplo.com",
            IdGradoEstudios = 1,
            IdFotoPerfil = 1
        };
        await _context.Alumno.AddAsync(alumnoExistente);
        await _context.SaveChangesAsync();

        var cambiarContrasenia = new CambiarContraseniaDTO
        {
            ContraseniaActual = "4anaka3",
            ContraseniaNueva = "1bela2"
        };

        await _servicioAlumno.CambiarContraseniaAsync(cambiarContrasenia, 57, crearHttpContext(57));
        var alumnoActualizado = await _alumnoDAO.ObtenerAlumnoPorIdAsync(57);

        Assert.NotNull(alumnoActualizado);
        Assert.Equal("1bela2", alumnoActualizado.Contrasenia);

        _context.Alumno.Remove(alumnoExistente);
        await _context.SaveChangesAsync();
    }

    //TC-18
    [Fact]
    public async Task CambiarContraseniaAlumnoFallido()
    {
        var alumnoExistente = new Alumno
        {
            IdAlumno = 57,
            NombreCompleto = "Nombre Ejemplo",
            NombreUsuario = "UsuarioEjemplo",
            Contrasenia = "4anaka3",
            Correo = "correo@ejemplo.com",
            IdGradoEstudios = 1,
            IdFotoPerfil = 1
        };
        await _context.Alumno.AddAsync(alumnoExistente);
        await _context.SaveChangesAsync();

        var cambiarContrasenia = new CambiarContraseniaDTO
        {
            ContraseniaActual = "44anaka3",
            ContraseniaNueva = "1bela2"
        };

        await Assert.ThrowsAsync<ContraseniaDiferenteException>(async ()=> await _servicioAlumno.CambiarContraseniaAsync(cambiarContrasenia, 57, crearHttpContext(57)));

        _context.Alumno.Remove(alumnoExistente);
        await _context.SaveChangesAsync();
    }
}
