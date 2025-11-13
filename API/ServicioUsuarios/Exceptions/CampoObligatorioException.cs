namespace ServicioUsuarios.Exceptions;

public class CampoObligatorioException : ArgumentException
{
    private string campo { get; }
    public CampoObligatorioException(string campo) : base($"El {campo} es nulo.") { }
}