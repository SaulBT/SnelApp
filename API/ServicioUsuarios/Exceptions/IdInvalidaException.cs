namespace ServicioUsuarios.Exceptions;

public class IdInvalidaException : ArgumentException
{
    private int idInvalida { get; }
    private string actor { get; }
    public IdInvalidaException(int idInvalida, string actor) : base($"La id '{idInvalida}' para el '{actor}' es inválida.")
    {
        this.idInvalida = idInvalida;
        this.actor = actor;
    }
}