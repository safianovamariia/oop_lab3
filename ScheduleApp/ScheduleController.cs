using ScheduleApp.Models;
using ScheduleApp.Search;
using ScheduleApp.Storage;

namespace ScheduleApp;

public class ScheduleController
{
    private const string NoFileMessage = "Open a JSON file first.";

    private string filePath = "";
    private Schedule? schedule;
    private readonly IScheduleStorage storage;
    private readonly List<ISearchCriterion> criteria = new();

    public ScheduleController(IScheduleStorage storage)
    {
        this.storage = storage;
    }

    public List<Session> OpenFile(string path)
    {
        schedule = storage.Load(path);
        filePath = path;
        criteria.Clear();
        return GetSessions();
    }

    public List<Session> GetSessions()
    {
        return (schedule ?? throw new InvalidOperationException(NoFileMessage)).Sessions.ToList();
    }

    public List<string> GetRoomNumbers()
    {
        return (schedule ?? throw new InvalidOperationException(NoFileMessage)).Rooms
            .Select(r => r.Number)
            .ToList();
    }

    public List<string> GetUserNames()
    {
        return (schedule ?? throw new InvalidOperationException(NoFileMessage)).Users
            .Select(u => u.FullName)
            .OrderBy(name => name)
            .ToList();
    }

    public bool AddSession(Session session)
    {
        Schedule current = schedule ?? throw new InvalidOperationException(NoFileMessage);
        if (!session.IsValid() || current.FindRoom(session.RoomNumber) == null || HasConflict(session, null))
            return false;

        current.AddSession(session);
        return true;
    }

    public bool UpdateSession(Session oldSession, Session newSession)
    {
        Schedule current = schedule ?? throw new InvalidOperationException(NoFileMessage);
        if (!newSession.IsValid() || current.FindRoom(newSession.RoomNumber) == null || HasConflict(newSession, oldSession))
            return false;

        return current.ReplaceSession(oldSession, newSession);
    }

    public bool DeleteSession(Session session)
    {
        return (schedule ?? throw new InvalidOperationException(NoFileMessage)).RemoveSession(session);
    }

    public bool SaveFile()
    {
        Schedule current = schedule ?? throw new InvalidOperationException(NoFileMessage);
        storage.Save(current, filePath);
        return true;
    }

    public bool AddCriterion(string field, string value)
    {
        value = value.Trim();
        switch (field)
        {
            case "room":
                criteria.RemoveAll(c => c is RoomCriterion);
                if (value != "")
                    criteria.Add(new RoomCriterion(value));
                return true;
            case "day":
                criteria.RemoveAll(c => c is DayCriterion);
                if (value != "")
                    criteria.Add(new DayCriterion(value));
                return true;
            case "person":
                criteria.RemoveAll(c => c is PersonCriterion);
                if (value != "")
                    criteria.Add(new PersonCriterion(value));
                return true;
            default:
                return false;
        }
    }

    public List<Session> Search()
    {
        List<Session> result = GetSessions();
        foreach (ISearchCriterion criterion in criteria)
            result = criterion.Apply(result);
        return result;
    }

    public void ClearCriteria()
    {
        criteria.Clear();
    }

    private bool HasConflict(Session session, Session? ignored)
    {
        return (schedule ?? throw new InvalidOperationException(NoFileMessage)).Sessions
            .Where(s => !ReferenceEquals(s, ignored))
            .Any(s => session.ConflictsWith(s));
    }
}
