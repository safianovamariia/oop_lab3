using System.Globalization;
using System.Text.Json.Serialization;

namespace ScheduleApp.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Lesson), "lesson")]
[JsonDerivedType(typeof(FreeAccess), "freeAccess")]
[JsonDerivedType(typeof(ArmWork), "armWork")]
public abstract class Session
{
    public string RoomNumber { get; }
    public string Day { get; }
    public string Time { get; }

    protected Session(string roomNumber, string day, string time)
    {
        RoomNumber = roomNumber ?? "";
        Day = day ?? "";
        Time = time ?? "";
    }

    public abstract string GetTypeName();

    public abstract string GetDetails();

    public abstract string GetPersonName();

    public virtual bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(RoomNumber)
               && Enum.TryParse<DayOfWeek>(Day, out _)
               && TimeOnly.TryParseExact(Time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }

    public virtual bool ConflictsWith(Session other)
    {
        return RoomNumber == other.RoomNumber && Day == other.Day && Time == other.Time;
    }

    public string GetInfo() =>
        $"{GetTypeName()} | room {RoomNumber} | {Day} {Time} | {GetDetails()} | {GetPersonName()}";
}
