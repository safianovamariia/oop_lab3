using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using ScheduleApp.Models;

namespace ScheduleApp.Storage;

public class JsonScheduleStorage : IScheduleStorage
{
    private readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        AllowOutOfOrderMetadataProperties = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    };

    public Schedule Load(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Schedule>(json, options)
               ?? throw new InvalidOperationException("The JSON file is empty.");
    }

    public void Save(Schedule schedule, string path)
    {
        string json = JsonSerializer.Serialize(schedule, options);
        File.WriteAllText(path, json);
    }
}
