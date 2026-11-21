namespace ScheduleApp.Models;

public class User
{
    public string FullName { get; set; } = "";
    public UserRole Role { get; set; }
    public string Department { get; set; } = "";

    public string GetInfo() => $"{FullName} ({Role}, {Department})";
}
