namespace Domain;

public class ExceptionPayload
{
    public string Message { get; set; }
    public string CustomPayload { get; set; }
    
    public ExceptionPayload(string message, string? customPayload = null)
    {
        Message = message;
        CustomPayload = customPayload ?? string.Empty;
    }
}