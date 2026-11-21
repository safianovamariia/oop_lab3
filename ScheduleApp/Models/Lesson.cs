using System.Text.Json.Serialization;

namespace ScheduleApp.Models;

public class Lesson : Session
{
    public string Subject { get; }
    public string Teacher { get; }
    public string Group { get; }

    [JsonConstructor]
    public Lesson(string roomNumber, string day, string time, string subject, string teacher, string group)
        : base(roomNumber, day, time)
    {
        Subject = subject ?? "";
        Teacher = teacher ?? "";
        Group = group ?? "";
    }

    public override string GetTypeName() => "Lesson";

    public override string GetDetails() => $"{Subject}, group {Group}";

    public override string GetPersonName() => Teacher;

    public override bool IsValid()
    {
        return base.IsValid()
               && !string.IsNullOrWhiteSpace(Subject)
               && !string.IsNullOrWhiteSpace(Teacher)
               && !string.IsNullOrWhiteSpace(Group);
    }
}
