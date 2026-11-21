namespace ScheduleApp.Models;

public class Schedule
{
    public string Faculty { get; set; } = "";
    public string Semester { get; set; } = "";
    public List<Room> Rooms { get; set; }
    public List<User> Users { get; set; }
    public List<Session> Sessions { get; set; }

    public Schedule()
    {
        Rooms = new List<Room>();
        Users = new List<User>();
        Sessions = new List<Session>();
    }

    public void AddSession(Session session)
    {
        Sessions.Add(session);
    }

    public bool RemoveSession(Session session)
    {
        return Sessions.Remove(session);
    }

    public bool ReplaceSession(Session oldSession, Session newSession)
    {
        int index = Sessions.IndexOf(oldSession);
        if (index < 0)
            return false;

        Sessions[index] = newSession;
        return true;
    }

    public Room? FindRoom(string number)
    {
        return Rooms.FirstOrDefault(r => r.Number == number);
    }
}
