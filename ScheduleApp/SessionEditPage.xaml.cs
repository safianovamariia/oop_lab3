using ScheduleApp.Models;

namespace ScheduleApp;

public partial class SessionEditPage : ContentPage
{
    private const string LessonType = "Lesson";
    private const string FreeAccessType = "Free access";
    private const string ArmWorkType = "ARM work";
    private static readonly string[] TimeSlots = { "08:40", "10:35", "12:20", "14:05", "15:50", "17:35" };

    private readonly List<string> roomNumbers;
    private readonly List<string> userNames;
    private readonly Session? original;
    private Session? result;

    public SessionEditPage(List<string> roomNumbers, List<string> userNames, Session? session)
    {
        InitializeComponent();
        this.roomNumbers = roomNumbers;
        this.userNames = userNames;
        original = session;

        TypePicker.ItemsSource = new List<string> { LessonType, FreeAccessType, ArmWorkType };
        RoomPicker.ItemsSource = this.roomNumbers;
        DayPicker.ItemsSource = Enumerable.Range(1, 7).Select(i => ((DayOfWeek)(i % 7)).ToString()).ToList();
        TimeSlotPicker.ItemsSource = TimeSlots.ToList();
        PersonPicker.ItemsSource = this.userNames;

        TitleLabel.Text = original == null ? "Add record" : "Edit record";
        if (original != null)
            ShowSession(original);
        else
            SelectSessionType(LessonType);
    }

    public void ShowSession(Session session)
    {
        EnterValue("type", session.GetTypeName());
        EnterValue("room", session.RoomNumber);
        EnterValue("day", session.Day);
        EnterValue("time", session.Time);
        EnterValue("person", session.GetPersonName());

        switch (session)
        {
            case Lesson lesson:
                EnterValue("subject", lesson.Subject);
                EnterValue("group", lesson.Group);
                break;
            case ArmWork arm:
                EnterValue("workplace", arm.Workplace);
                break;
        }
    }

    private void OnTypeChanged(object? sender, EventArgs e) =>
        SelectSessionType(TypePicker.SelectedItem?.ToString() ?? LessonType);

    public void SelectSessionType(string type)
    {
        SetPickerValue(TypePicker, type);
        LessonFields.IsVisible = type == LessonType;
        ArmFields.IsVisible = type == ArmWorkType;
        PersonLabel.Text = type switch
        {
            LessonType => "Teacher",
            ArmWorkType => "User",
            _ => "Responsible"
        };
    }

    public void EnterValue(string field, string value)
    {
        switch (field)
        {
            case "type": SelectSessionType(value); break;
            case "room": SetPickerValue(RoomPicker, value); break;
            case "day": SetPickerValue(DayPicker, value); break;
            case "time": SetPickerValue(TimeSlotPicker, value); break;
            case "person": SetPickerValue(PersonPicker, value); break;
            case "subject": SubjectEntry.Text = value; break;
            case "group": GroupEntry.Text = value; break;
            case "workplace": WorkplaceEntry.Text = value; break;
        }
    }

    public async void BtnClickSave(object? sender, EventArgs e)
    {
        Session session = BuildSession();
        if (!session.IsValid())
        {
            ShowMessage("Fill in all fields.");
            return;
        }

        result = session;
        await Navigation.PopModalAsync();
    }

    public async void BtnClickCancel(object? sender, EventArgs e)
    {
        result = null;
        await Navigation.PopModalAsync();
    }

    // Completes when the modal form closes; null means the user cancelled.
    public Task<Session?> GetResult()
    {
        var completion = new TaskCompletionSource<Session?>();
        Disappearing += (_, _) => completion.TrySetResult(result);
        return completion.Task;
    }

    public void ShowMessage(string text)
    {
        MessageLabel.Text = text;
    }

    private Session BuildSession()
    {
        string room = SelectedValue(RoomPicker);
        string day = SelectedValue(DayPicker);
        string time = SelectedValue(TimeSlotPicker);
        string person = SelectedValue(PersonPicker);

        return SelectedValue(TypePicker) switch
        {
            FreeAccessType => new FreeAccess(room, day, time, person),
            ArmWorkType => new ArmWork(room, day, time, (WorkplaceEntry.Text ?? "").Trim(), person),
            _ => new Lesson(room, day, time, (SubjectEntry.Text ?? "").Trim(), person, (GroupEntry.Text ?? "").Trim())
        };
    }

    private static string SelectedValue(Picker picker) => picker.SelectedItem?.ToString() ?? "";

    private static void SetPickerValue(Picker picker, string value)
    {
        if (picker.ItemsSource is not List<string> items || value == "")
            return;

        if (!items.Contains(value))
        {
            items.Add(value);
            picker.ItemsSource = null;
            picker.ItemsSource = items;
        }

        picker.SelectedIndex = items.IndexOf(value);
    }
}
