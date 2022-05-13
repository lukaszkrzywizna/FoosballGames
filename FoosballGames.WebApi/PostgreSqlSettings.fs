namespace FoosballGames.WebApi

open Microsoft.FSharp.Core
open Npgsql

[<CLIMutable>]
type PostgreSqlSettings =
    {Host: string; Port: int; Database: string; Username: string; Password: string}

    member this.BuildConnectionString() =
        let builder =
            NpgsqlConnectionStringBuilder(
                Host = this.Host,
                Port = this.Port,
                Database = this.Database,
                Username = this.Username,
                Password = this.Password
            )

        builder.ConnectionString
