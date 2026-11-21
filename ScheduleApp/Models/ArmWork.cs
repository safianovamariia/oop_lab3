using System.Text.Json.Serialization;

namespace ScheduleApp.Models;

public class ArmWork : Session
{
    public string Workplace { get; }
    public string User { get; }

    [JsonConstructor]
    public ArmWork(string roomNumber, string day, string time, string workplace, string user)
        : base(roomNumber, day, time)
    {
        Workplace = workplace ?? "";
        User = user ?? "";
    }

    public override string GetTypeName() => "ARM work";

    public override string GetDetails() => $"Workplace {Workplace}";

    public override string GetPersonName() => User;

    public override bool IsValid()
    {
        return base.IsValid()
               && !string.IsNullOrWhiteSpace(Workplace)
               && !string.IsNullOrWhiteSpace(User);
    }

    // Different workplaces of one ARM room can be used at the same time.
    public override bool ConflictsWith(Session other)
    {
        if (other is ArmWork arm)
            return base.ConflictsWith(other) && Workplace == arm.Workplace;
        return base.ConflictsWith(other);
    }
}
