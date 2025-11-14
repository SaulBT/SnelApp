namespace ServicioUsuarios.Exceptions;

public class DiscordanciaDeIdException : Exception
{
    private int idRecurso { get; }
    private int idProporcionado { get; }
    private string actor{ get; }
    public DiscordanciaDeIdException(int idRecurso, int idProporcionado, string actor)
        : base($"El ID del '{actor}' ({idRecurso}) no coincide con el ID proporcionado ({idProporcionado}).")
    {
        this.idRecurso = idRecurso;
        this.idProporcionado = idProporcionado;
        this.actor = actor;
     }
}