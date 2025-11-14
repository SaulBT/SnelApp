using Microsoft.EntityFrameworkCore;
using ServicioUsuarios.Services.Interfaces;
using ServicioUsuarios.Services.Implementation;
using ServicioUsuarios.Middlewares;
using ServicioUsuarios.Data.DAOs.Interfaces;
using ServicioUsuarios.Data.DAOs.Implementation;
using ServicioUsuarios.Data.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ServicioUsuarios.Validations;
using ServicioUsuarios.Data.DTOs.Alumno;
using ServicioUsuarios.Data;
using ServicioUsuarios.Data.DTOs.Instructor;
using ServicioUsuarios.Data.DTOs.Catalogo;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UsuariosDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IServicioAlumno, ServicioAlumno>();
builder.Services.AddScoped<IAlumnoDAO, AlumnoDAO>();
builder.Services.AddScoped<IServicioInstructor, ServicioInstructor>();
builder.Services.AddScoped<IInstructorDAO, InstructorDAO>();
builder.Services.AddScoped<ServicioCatalogo, ServicioCatalogo>();
builder.Services.AddScoped<IGradoEstudiosDAO, GradoEstudiosDAO>();
builder.Services.AddScoped<IGradoProfesionalDAO, GradoProfesionalDAO>();
builder.Services.AddScoped<IServicioLogin, ServicioLogin>();
builder.Services.AddScoped<GeneradorToken>();
builder.Services.AddScoped<AlumnoValidaciones>();
builder.Services.AddScoped<InstructorValidaciones>();
builder.Services.AddScoped<LoginValidaciones>();

builder.Services.AddAuthentication(options => {    
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    
}).AddJwtBearer(options => {
    var config = builder.Configuration;
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    var llave = config["JWTSettings:Key"];
    options.TokenValidationParameters = new TokenValidationParameters() {
        ValidIssuer = config["JWTSettings:Issuer"],
        ValidAudience = config["JWTSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(llave)),        
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseManejoExcepciones();
app.UseAuthentication();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();
    app.UseAuthorization();
    app.UseAuthentication();
}

/*
 * ServicioAlumno
*/

//Validar datos de registro de alumno
app.MapPost("/alumnos/validar", async (RegistrarAlumnoDTO registrarAlumnoDto, IServicioAlumno servicio) =>
{
    await servicio.ValidarDatosRegistroAsync(registrarAlumnoDto);
    return Results.Accepted();
})
.WithName("Validar registro Alumno")
.WithTags("Alumnos")
.WithSummary("Validar datos de registro de un Alumno")
.WithDescription(
    "Valida los siguientes datos: " +
    "\n - Nombre completo." +
    "\n - Nombre usuario." +
    "\n - Correo electrónico." +
    "\n - Contraseña." +
    "\n - Id del último grado de estudios obtenido.")
.Accepts<RegistrarAlumnoDTO>("application/json")
.Produces(202)
.Produces(400)
.Produces(409)
.WithOpenApi();

//Registrar alumno
app.MapPost("/alumnos", async (RegistrarAlumnoDTO alumnoNuevoDto, IServicioAlumno servicio) =>
{
    await servicio.RegistrarAsync(alumnoNuevoDto);
    return Results.Created();
})
.WithName("Registrar Alumno")
.WithTags("Alumnos")
.WithSummary("Registrar un nuevo Alumno en el sistema")
.WithDescription(
    "Crea un Alumno con los datos: " +
    "\n - Nombre completo." +
    "\n - Nombre de usuario." +
    "\n - Correo electrónico." +
    "\n - Contraseña." +
    "\n - Id del último grado de estudios cursado.")
.Accepts<RegistrarAlumnoDTO>("application/json")
.Produces(201)
.Produces(400)
.Produces(409)
.WithOpenApi();

//Obtener alumno por id
app.MapGet("/alumnos/{idAlumno}", async (int idAlumno, IServicioAlumno servicio) =>
{
    var alumno = await servicio.ObtenerAlumnoPorIdAsync(idAlumno);
    return Results.Ok(alumno);
})
.WithName("Obtener Alumno por id")
.WithTags("Alumnos")
.WithSummary("Obtiene un ALumno por medio de su id")
.WithDescription(
    "Obtiene un Alumno con los datos: " +
    "\n - Id del Alumno" +
    "\n - Nombre completo." +
    "\n - Nombre de usuario." +
    "\n - Correo electrónico." +
    "\n - Id del último grado de estudios cursado." +
    "\n - Id de la foto de perfil." +
    "\nCon la id del Alumno.")
.Produces<AlumnoDTO>(200)
.Produces(400)
.Produces(404)
.Produces(409)
.WithOpenApi();

//Actualizar alumno
app.MapPut("/alumnos/{idAlumno}", async (HttpContext context, int idAlumno, ActualizarAlumnoDTO alumnoActualizadoDTO, IServicioAlumno servicio) =>
{
    var alumno = await servicio.ActualizarAsync(context, idAlumno, alumnoActualizadoDTO);
    return Results.Accepted("/alumnos/{idAlumno}",alumno);
})
.WithName("Actualizar Alumno")
.WithTags("Alumnos")
.WithSummary("Actualiza un Alumno con la id")
.WithDescription(
    "Actualiza un Alumno recibiendo lo siguientes datos:" +
    "\n - Id del Alumno" +
    "\n - Nombre completo" +
    "\n - Nombre de usuario" +
    "\n - Id del último grado de estudios" +
    "\n - Id de la nueva foto de perfil." +
    "\nY devuelve los siguientes datos:" +
    "\n - Id del Alumno" +
    "\n - Nombre completo" +
    "\n - Nombre del usuario" +
    "\n - Correo electrónico" +
    "\n - Id del último grado de estudios" +
    "\n - Id de la nueva foto de perfil.")
.Accepts<ActualizarAlumnoDTO>("application/json")
.Produces(204)
.Produces(400)
.Produces(401)
.Produces(404)
.Produces(409)
.RequireAuthorization()
.WithOpenApi();

//Eliminar alumno
app.MapDelete("/alumnos/{idAlumno}", async (HttpContext context, int idAlumno, IServicioAlumno servicio) =>
{
    await servicio.EliminarAsync(context, idAlumno);
    return Results.Accepted();
})
.WithName("Eliminar Alumno")
.WithTags("Alumnos")
.WithSummary("Elimina un Alumno con la id")
.WithDescription("Elimina un Alumno con la id")
.Produces(204)
.Produces(400)
.Produces(401)
.Produces(404)
.RequireAuthorization()
.WithOpenApi();

//Cambiar contrasenia de alumno
app.MapPut("/alumnos/{idAlumno}/contrasenia", async (CambiarContraseniaDTO cambiarContraseniaDto, int idAlumno, IServicioAlumno servicio, HttpContext context) =>
{
    await servicio.CambiarContraseniaAsync(cambiarContraseniaDto, idAlumno, context);
    return Results.Accepted();
})
.WithName("Cambiar contrasenia de Alumno")
.WithTags("Alumnos")
.WithSummary("Cambia la contraseña de un Alumno")
.WithDescription("Cambia la contraseña de un Alumno")
.Accepts<CambiarContraseniaDTO>("application/json")
.Produces(204)
.Produces(400)
.Produces(401)
.Produces(409)
.RequireAuthorization()
.WithOpenApi();

/*
 * ServicioInstructor
*/

//Validar datos de registro de instructor
app.MapPost("/instructores/validar", async (RegistrarInstructorDTO instructorNuevoDto, IServicioInstructor servicio) =>
{
    await servicio.ValidarDatosRegistroAsync(instructorNuevoDto);
    return Results.Accepted();
})
.WithName("Validar registro Instructor")
.WithTags("Instructores")
.WithSummary("Validar datos de registro de un Instructor")
.WithDescription(
    "Valida los siguientes datos: " +
    "\n - Nombre completo." +
    "\n - Nombre usuario." +
    "\n - Correo electrónico." +
    "\n - Contraseña." +
    "\n - Id del último grado profesional obtenido.")
.Accepts<RegistrarInstructorDTO>("application/json")
.Produces(202)
.Produces(400)
.Produces(409)
.WithOpenApi();
//Registrar instructor
app.MapPost("/instructores", async (RegistrarInstructorDTO instructorNuevoDto, IServicioInstructor servicio) =>
{
    await servicio.RegistrarAsync(instructorNuevoDto);
    return Results.Created();
})
.WithName("Registrar Instructor")
.WithTags("Instructores")
.WithSummary("Registrar un nuevo Instructor en el sistema")
.WithDescription(
    "Crea un Instructor con los datos: " +
    "\n - Nombre completo." +
    "\n - Nombre usuario." +
    "\n - Correo electrónico." +
    "\n - Contraseña." +
    "\n - Id del último grado profesional obtenido.")
.Accepts<RegistrarInstructorDTO>("application/json")
.Produces(201)
.Produces(400)
.Produces(409)
.WithOpenApi();

//Obtener instructor por id
app.MapGet("/instructores/{idInstructor}", async (int idInstructor, IServicioInstructor servicio) =>
{
    var instructor = await servicio.ObtenerInstructorPorIdAsync(idInstructor);
    return Results.Ok(instructor);
})
.WithName("Obtener Instructor por id")
.WithTags("Instructores")
.WithSummary("Obtiene un Instructor por medio de su id")
.WithDescription(
    "Obtiene un Instructor con los datos: " +
    "\n - Id del Instructor" +
    "\n - Nombre completo." +
    "\n - Nombre usuario." +
    "\n - Correo electrónico." +
    "\n - Id del último grado de estudios cursado." +
    "\n - Id de la foto de perfil." +
    "\nCon la id del Instructor.")
.Produces<InstructorDTO>(200)
.Produces(400)
.Produces(404)
.Produces(409)
.WithOpenApi();

//Actualizar instructor
app.MapPut("/instructores/{idInstructores}", async (HttpContext context, int idInstructor, ActualizarInstructorDTO instructorActualizadoDTO, IServicioInstructor servicio) =>
{
    var instructor = await servicio.ActualizarAsync(context, idInstructor, instructorActualizadoDTO);
    return Results.Accepted("/instructores/{idInstructor}", instructor);
})
.WithName("Actualizar Instructor")
.WithTags("Instructores")
.WithSummary("Actualiza un Instructor con la id")
.WithDescription(
    "Actualiza un Instructor recibiendo lo siguientes datos:" +
    "\n - Id del Instructor" +
    "\n - Nombre completo" +
    "\n - Nombre de usuario" +
    "\n - Id del último grado de estudios cursado." +
    "\nY devuelve los siguientes datos:" +
    "\n - Id del Instructor" +
    "\n - Nombre completo" +
    "\n - Nombre del usuario" +
    "\n - Correo electrónico" +
    "\n - Id del último grado de estudios cursado.")
.Accepts<ActualizarInstructorDTO>("application/json")
.Produces(204)
.Produces(400)
.Produces(401)
.Produces(404)
.Produces(409)
.RequireAuthorization()
.WithOpenApi();

//Eliminar instructor
app.MapDelete("/instructor/{idInstructor}", async (HttpContext context, int idInstructor, IServicioInstructor servicio) =>
{
    await servicio.EliminarAsync(context, idInstructor);
    return Results.Accepted();
})
.WithName("EliminarInstructor")
.WithTags("Instructores")
.WithSummary("Elimina un Instructor con la id")
.WithDescription("Elimina un Instructor con la id")
.Produces(204)
.Produces(400)
.Produces(401)
.Produces(404)
.RequireAuthorization()
.WithOpenApi();

//Cambiar contrasenia de instructor
app.MapPut("/instructores/{idInstructores}/contrasenia", async (CambiarContraseniaDTO cambiarContraseniaDto, int idInstructor, IServicioInstructor servicio, HttpContext context) =>
{
    await servicio.CambiarContraseniaAsync(cambiarContraseniaDto, idInstructor, context);
    return Results.Accepted();
})
.WithName("Cambiar contraseña de Instructor")
.WithTags("Instructores")
.WithSummary("Cambia la contraseña de un Instructor")
.WithDescription("Cambia la contraseña de un Instructor")
.Accepts<CambiarContraseniaDTO>("application/json")
.Produces(204)
.Produces(400)
.Produces(401)
.Produces(409)
.RequireAuthorization()
.WithOpenApi();

/*
 * ServicioCatalogo
*/
//Obtener catalogos de estudios
app.MapGet("/catalogos/grados-estudios", async (ServicioCatalogo servicio) =>
{
    var gradosEstudios = await servicio.ObtenerGradosEstudiosAsync();
    return Results.Ok(gradosEstudios);
})
.WithName("Obtener Grados de Estudios")
.WithTags("Catalogos")
.WithSummary("Obtener Grados de Estudios")
.WithDescription(
    "Obtener grados de estudios:" +
    "\n - Primario" +
    "\n - Secundaria" +
    "\n - Bachillerato" +
    "\n - Universidad" +
    "\n - Maestría" +
    "\n - Doctorado" +
    "\n - Postdoctorado")
.Produces<List<GradoEstudiosDTO>>(200)
.Produces(404)
.WithOpenApi();

//Obtener catalogos de grados profesionales
app.MapGet("/catalogos/grados-profesionales", async (ServicioCatalogo servicio) =>
{
    var gradosProfesionales = await servicio.ObtenerGradosProfesionalesAsync();
    return Results.Ok(gradosProfesionales);
})
.WithName("Obtener Grados Profesionales")
.WithTags("Catalogos")
.WithSummary("Obtener Grados Profesionales")
.WithDescription(
    "Obtener grados profesionales:" +
    "\n - Licenciatura" +
    "\n - Maestría" +
    "\n - Doctorado")
.Produces<List<GradoProfesionalDTO>>(200)
.Produces(404)
.WithOpenApi();

//Obtener grado de estudio por id
app.MapGet("/catalogos/grado-estudio/{idGradoEstudios}", async (int idGradoEstudios, ServicioCatalogo servicio) =>
{
    var gradoEstudio = await servicio.ObtenerGradoEstudioPorIdAsync(idGradoEstudios);
    return Results.Ok(gradoEstudio);
})
.WithName("Obtener un Grado de Estudio por id")
.WithTags("Catalogos")
.WithSummary("Obtener un Grado de Estudio")
.WithDescription("Obtener un Grado de Estudio")
.Produces<GradoEstudiosDTO>(200)
.Produces(404)
.WithOpenApi();

//Obtener grado profesional por id
app.MapGet("/catalogos/grado-profesional/{idGradoProfesional}", async (int idGradoProfesional, ServicioCatalogo servicio) =>
{
    var gradoProfesional = await servicio.ObtenerGradoProfesionalPorIdAsync(idGradoProfesional);
    return Results.Ok(gradoProfesional);
})
.WithName("Obtener un Grado Profesional por id")
.WithTags("Catalogos")
.WithSummary("Obtener un grado profesional")
.WithDescription("Obtener un grado profesional")
.Produces<GradoProfesionalDTO>(200)
.Produces(404)
.WithOpenApi();

/*
 * ServicioLogin
*/
app.MapPost("/login", async (IniciarSesionDTO usuarioDto, IServicioLogin servicio) =>
{
    var resultado = await servicio.IniciarSesion(usuarioDto);
    return Results.Ok(resultado);
})
.WithName("Iniciar Sesion")
.WithTags("Login")
.WithSummary("Logearse como Alumno o Docente")
.WithDescription(
    "Logearse con los siguientes datos:" +
    "\n - Tipo de usuario" +
    "\n - Nombre de usuario o correo electrónico" +
    "\n - Contraseña")
.Produces<AlumnoDTO>(200)
.Produces<InstructorDTO>(200)
.Produces(400)
.Produces(404)
.WithOpenApi();

app.UseHttpsRedirection();

app.Run();
