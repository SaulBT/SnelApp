namespace ServicioUsuarios.Exceptions;

public class RecursoYaExistenteException : Exception
{
    private string recurso { get; }
    public RecursoYaExistenteException(string recurso) : base($"'{recurso}' ya está en uso.")
    {
        this.recurso = recurso;
    }
}