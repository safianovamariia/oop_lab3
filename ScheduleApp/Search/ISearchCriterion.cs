using ScheduleApp.Models;

namespace ScheduleApp.Search;

public interface ISearchCriterion
{
    List<Session> Apply(List<Session> sessions);
}
