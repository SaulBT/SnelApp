namespace ServicioUsuarios.Exceptions;

public class RecursoNoEncontradoException : Exception
{
    private string recurso { get; }
    public RecursoNoEncontradoException(string recurso) : base($"No se pudo encontrar al recurso '{recurso}' en la base de datos.")
    {
        this.recurso = recurso;
    }
}