using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Npgsql;

string redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "redis";
string redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";

string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "db";
string dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "iplpredictions";

var redis = await ConnectionMultiplexer.ConnectAsync($"{redisHost}:{redisPort}");
var dbRedis = redis.GetDatabase();

var connString = $"Host={dbHost};Username={dbUser};Password={dbPassword};Database={dbName}";
await using var pg = new NpgsqlConnection(connString);
await pg.OpenAsync();

// create table if not exists
var createSql = @"
CREATE TABLE IF NOT EXISTS predictions (
    team text PRIMARY KEY,
    count integer NOT NULL
);";
await using (var cmd = new NpgsqlCommand(createSql, pg))
{
    await cmd.ExecuteNonQueryAsync();
}

Console.WriteLine("Worker started, waiting for predictions...");

while (true)
{
    // BLPOP-like behavior using ListLeftPop + delay
    var value = await dbRedis.ListLeftPopAsync("ipl_predictions");
    if (!value.IsNullOrEmpty)
    {
        var team = value.ToString();
        Console.WriteLine($"Processing prediction for {team}");

        var upsertSql = @"
INSERT INTO predictions (team, count) VALUES (@team, 1)
ON CONFLICT (team) DO UPDATE SET count = predictions.count + 1;";
        await using var upsert = new NpgsqlCommand(upsertSql, pg);
        upsert.Parameters.AddWithValue("team", team);
        await upsert.ExecuteNonQueryAsync();
    }
    else
    {
        await Task.Delay(500); // sleep when queue empty
    }
}

