using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ServicioUsuarios.Data;
using ServicioUsuarios.Data.DAOs.Implementation;
using ServicioUsuarios.Data.DTOs.Catalogo;
using ServicioUsuarios.Models;
using ServicioUsuarios.Services.Implementation;

public class CatalogoPruebas
{
    private readonly UsuariosDbContext _context;
    private readonly GradoEstudiosDAO _gradoEstudiosDAO;
    private readonly GradoProfesionalDAO _gradoProesionalDAO;
    private readonly ServicioCatalogo _servicioCatalogo;
    private readonly ILogger<ServicioCatalogo> _logger;

    public CatalogoPruebas()
    {
        var opciones = new DbContextOptionsBuilder<UsuariosDbContext>()
            .UseInMemoryDatabase(databaseName: "UsuariosBDPruebas")
            .Options;
        _context = new UsuariosDbContext(opciones);

        _gradoEstudiosDAO = new GradoEstudiosDAO(_context);
        _gradoProesionalDAO = new GradoProfesionalDAO(_context);

        _logger = NullLogger<ServicioCatalogo>.Instance;

        _servicioCatalogo = new ServicioCatalogo(_gradoEstudiosDAO, _gradoProesionalDAO, _logger);
    }

    //TC-13
    [Fact]
    public async Task ObtenerGradosDeEstudiosAsync()
    {
        var grado1 = new GradoEstudios
        {
            IdGradoEstudios = 1,
            Nombre = "Primaria"
        };
        _context.GradoEstudios.AddAsync(grado1);
        var grado2 = new GradoEstudios
        {
            IdGradoEstudios = 2,
            Nombre = "Secundaria"
        };
        _context.GradoEstudios.AddAsync(grado2);
        var grado3 = new GradoEstudios
        {
            IdGradoEstudios = 3,
            Nombre = "Bachillerato"
        };
        _context.GradoEstudios.AddAsync(grado3);
        var grado4 = new GradoEstudios
        {
            IdGradoEstudios = 4,
            Nombre = "Universidad"
        };
        _context.GradoEstudios.AddAsync(grado4);
        var grado5 = new GradoEstudios
        {
            IdGradoEstudios = 5,
            Nombre = "Maestría"
        };
        _context.GradoEstudios.AddAsync(grado5);
        var grado6 = new GradoEstudios
        {
            IdGradoEstudios = 6,
            Nombre = "Doctorado"
        };
        _context.GradoEstudios.AddAsync(grado6);
        _context.SaveChangesAsync();

        List<GradoEstudiosDTO> grados = await _servicioCatalogo.ObtenerGradosEstudiosAsync();

        Assert.NotNull(grados);
        Assert.True(grados.Count > 0);
        Assert.Equal("Primaria",grados[0].Nombre);
        Assert.Equal("Secundaria",grados[1].Nombre);
        Assert.Equal("Bachillerato",grados[2].Nombre);
        Assert.Equal("Universidad",grados[3].Nombre);
        Assert.Equal("Maestría",grados[4].Nombre);
        Assert.Equal("Doctorado",grados[5].Nombre);
    }

    //TC-14
    [Fact]
    public async Task ObtenerGradosProfesionalesAsync()
    {
        var grado1 = new GradoProfesional
        {
            IdGradoProfesional = 1,
            Nombre = "Licenciatura"
        };
        _context.GradoProfesional.AddAsync(grado1);
        var grado2 = new GradoProfesional
        {
            IdGradoProfesional = 2,
            Nombre = "Maestría"
        };
        _context.GradoProfesional.AddAsync(grado2);
        var grado3 = new GradoProfesional
        {
            IdGradoProfesional = 3,
            Nombre = "Doctorado"
        };
        _context.GradoProfesional.AddAsync(grado3);
        _context.SaveChangesAsync();

        List<GradoProfesionalDTO> grados = await _servicioCatalogo.ObtenerGradosProfesionalesAsync();

        Assert.NotNull(grados);
        Assert.True(grados.Count > 0);
        Assert.Equal("Licenciatura",grados[0].Nombre);
        Assert.Equal("Maestría",grados[1].Nombre);
        Assert.Equal("Doctorado",grados[2].Nombre);
    }
}