using Npgsql;

namespace FoosballGames.WebApi;

public class PostgreSqlSettings
{
    public string? Host { get; set; }
    public int Port { get; set; } = 5432;
    public string? Database { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

    public string BuildConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = Database,
            Username = Username,
            Password = Password
        };

        return builder.ConnectionString;
    }
}