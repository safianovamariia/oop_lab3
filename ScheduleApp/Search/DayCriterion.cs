using ScheduleApp.Models;

namespace ScheduleApp.Search;

public class DayCriterion : ISearchCriterion
{
    private readonly string day;

    public DayCriterion(string day)
    {
        this.day = day;
    }

    public List<Session> Apply(List<Session> sessions)
    {
        return sessions.Where(s => s.Day.Equals(day, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
