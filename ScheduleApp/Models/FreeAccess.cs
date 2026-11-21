using System.Text.Json.Serialization;

namespace ScheduleApp.Models;

public class FreeAccess : Session
{
    public string Responsible { get; }

    [JsonConstructor]
    public FreeAccess(string roomNumber, string day, string time, string responsible)
        : base(roomNumber, day, time)
    {
        Responsible = responsible ?? "";
    }

    public override string GetTypeName() => "Free access";

    public override string GetDetails() => "Free access time";

    public override string GetPersonName() => Responsible;

    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrWhiteSpace(Responsible);
    }
}
