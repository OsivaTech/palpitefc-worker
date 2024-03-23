namespace PalpiteFC.Worker.Repository.Settings;

public class DbSettings
{
    public string? Server { get; set; }
    public string? UserId { get; set; }
    public string? Database { get; set; }
    public string? Password { get; set; }

    public string ToConnectionString() => $"Server={Server}; Database={Database}; Uid={UserId}; Pwd={Password};";
}
