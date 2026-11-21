using ScheduleApp.Models;

namespace ScheduleApp.Search;

public class RoomCriterion : ISearchCriterion
{
    private readonly string roomNumber;

    public RoomCriterion(string roomNumber)
    {
        this.roomNumber = roomNumber;
    }

    public List<Session> Apply(List<Session> sessions)
    {
        return sessions.Where(s => s.RoomNumber == roomNumber).ToList();
    }
}
