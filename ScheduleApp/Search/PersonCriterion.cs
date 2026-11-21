using ScheduleApp.Models;

namespace ScheduleApp.Search;

public class PersonCriterion : ISearchCriterion
{
    private readonly string personName;

    public PersonCriterion(string personName)
    {
        this.personName = personName;
    }

    public List<Session> Apply(List<Session> sessions)
    {
        return sessions.Where(s => s.GetPersonName().Contains(personName, StringComparison.OrdinalIgnoreCase))
                       .ToList();
    }
}
