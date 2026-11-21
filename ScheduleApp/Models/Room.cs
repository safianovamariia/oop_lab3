namespace ScheduleApp.Models;

public class Room
{
    public string Number { get; set; } = "";
    public string Building { get; set; } = "";
    public RoomType Type { get; set; }
    public string Department { get; set; } = "";
    public int WorkplacesCount { get; set; }

    public string GetInfo()
    {
        string type = Type == RoomType.DisplayClass ? "display class" : "department ARM";
        return $"Room {Number} (building {Building}), {type}, {Department}, workplaces: {WorkplacesCount}";
    }
}
