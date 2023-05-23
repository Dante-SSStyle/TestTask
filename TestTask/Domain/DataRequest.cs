namespace TestTask.Domain;

/// <summary>
/// This model represents DTO object for the JSON acception with base64 encoded binary data.
/// </summary>
public class DataRequest
{
    public string? Data { get; set; }

    public DataRequest(string data)
    {
        Data = data;
    }
}