using ScheduleApp.Models;

namespace ScheduleApp.Storage;

public interface IScheduleStorage
{
    Schedule Load(string path);

    void Save(Schedule schedule, string path);
}
