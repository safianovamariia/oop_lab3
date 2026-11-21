using System.Text.Json;
using ScheduleApp.Models;
using ScheduleApp.Storage;

namespace ScheduleApp;

public partial class MainPage : ContentPage
{
    private const string AllValues = "All";
    private static readonly string[] Headers = { "Type", "Room", "Day", "Time", "Details", "Teacher / user" };
    private static readonly Color HeaderColor = Color.FromArgb("#E6E6E6");
    private static readonly Color RowColor = Colors.White;
    private static readonly Color SelectedColor = Color.FromArgb("#CFE3FF");

    private readonly ScheduleController controller = new(new JsonScheduleStorage());
    private Session? selectedSession;
    private bool isExitDialogOpen;

    internal static bool ExitConfirmed { get; private set; }

    public MainPage()
    {
        InitializeComponent();

        List<string> days = Enumerable.Range(1, 7).Select(i => ((DayOfWeek)(i % 7)).ToString()).ToList();
        FillPicker(DayPicker, days);
        ShowSessions(new List<Session>());
    }

    public async void BtnClickOpenFile(object? sender, EventArgs e)
    {
        var options = new PickOptions
        {
            PickerTitle = "Choose the JSON file with the schedule",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                [DevicePlatform.WinUI] = new[] { ".json" }
            })
        };

        FileResult? file = await FilePicker.Default.PickAsync(options);
        if (file != null)
            SelectFile(file.FullPath);
    }

    public void SelectFile(string path)
    {
        try
        {
            List<Session> sessions = controller.OpenFile(path);
            FillPicker(RoomPicker, controller.GetRoomNumbers());
            ResetPicker(DayPicker);
            PersonEntry.Text = "";
            selectedSession = null;
            ShowSessions(sessions);
            ShowMessage($"File opened: {Path.GetFileName(path)}. Records: {sessions.Count}");
        }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException
                                      or InvalidOperationException or NotSupportedException)
        {
            ShowMessage($"Cannot open the file: {ex.Message}");
        }
    }

    public void BtnClickSave(object? sender, EventArgs e)
    {
        try
        {
            if (controller.SaveFile())
                ShowMessage("Schedule saved to the file.");
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            ShowMessage($"Cannot save the file: {ex.Message}");
        }
    }

    public void ShowSessions(List<Session> sessions)
    {
        SessionsGrid.Children.Clear();
        SessionsGrid.RowDefinitions.Clear();

        SessionsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        for (int column = 0; column < Headers.Length; column++)
            SessionsGrid.Add(CreateCell(Headers[column], null), column, 0);

        int row = 1;
        foreach (Session session in sessions)
        {
            SessionsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            string[] values =
            {
                session.GetTypeName(), session.RoomNumber, session.Day, session.Time,
                session.GetDetails(), session.GetPersonName()
            };
            for (int column = 0; column < values.Length; column++)
                SessionsGrid.Add(CreateCell(values[column], session), column, row);
            row++;
        }

        HighlightSelected();
    }

    public void SelectSession(Session session)
    {
        selectedSession = session;
        HighlightSelected();
        ShowMessage($"Selected: {session.GetInfo()}");
    }

    private void RefreshGrid()
    {
        try
        {
            ShowSessions(controller.Search());
        }
        catch (InvalidOperationException)
        {
            ShowSessions(new List<Session>());
        }
    }

    public async void BtnClickAdd(object? sender, EventArgs e)
    {
        try
        {
            var page = new SessionEditPage(controller.GetRoomNumbers(), controller.GetUserNames(), null);
            await Navigation.PushModalAsync(page);
            Session? created = await page.GetResult();
            if (created == null)
            {
                ShowMessage("Adding cancelled.");
                return;
            }

            if (controller.AddSession(created))
            {
                selectedSession = created;
                RefreshGrid();
                ShowMessage("Record added.");
            }
            else
            {
                ShowMessage(ConflictMessage(created));
            }
        }
        catch (InvalidOperationException ex)
        {
            ShowMessage(ex.Message);
        }
    }

    public async void BtnClickEdit(object? sender, EventArgs e)
    {
        if (selectedSession == null)
        {
            ShowMessage("Select a record in the table first.");
            return;
        }

        try
        {
            Session oldSession = selectedSession;
            var page = new SessionEditPage(controller.GetRoomNumbers(), controller.GetUserNames(), oldSession);
            await Navigation.PushModalAsync(page);
            Session? edited = await page.GetResult();
            if (edited == null)
            {
                ShowMessage("Editing cancelled.");
                return;
            }

            if (controller.UpdateSession(oldSession, edited))
            {
                selectedSession = edited;
                RefreshGrid();
                ShowMessage("Record updated.");
            }
            else
            {
                ShowMessage(ConflictMessage(edited));
            }
        }
        catch (InvalidOperationException ex)
        {
            ShowMessage(ex.Message);
        }
    }

    public async void BtnClickDelete(object? sender, EventArgs e)
    {
        if (selectedSession == null)
        {
            ShowMessage("Select a record in the table first.");
            return;
        }

        if (!await ConfirmDelete())
            return;

        try
        {
            controller.DeleteSession(selectedSession);
            selectedSession = null;
            RefreshGrid();
            ShowMessage("Record deleted.");
        }
        catch (InvalidOperationException ex)
        {
            ShowMessage(ex.Message);
        }
    }

    public async Task<bool> ConfirmDelete()
    {
        return await DisplayAlertAsync("Delete",
            $"Delete this record?\n{selectedSession?.GetInfo()}", "Yes", "No");
    }

    private void OnRoomChanged(object? sender, EventArgs e) =>
        SelectCriterion("room", GetSelectedValue(RoomPicker));

    private void OnDayChanged(object? sender, EventArgs e) =>
        SelectCriterion("day", GetSelectedValue(DayPicker));

    private void OnPersonChanged(object? sender, TextChangedEventArgs e) =>
        SelectCriterion("person", e.NewTextValue ?? "");

    public void SelectCriterion(string field, string value)
    {
        controller.AddCriterion(field, value);
    }

    public void BtnClickSearch(object? sender, EventArgs e)
    {
        try
        {
            List<Session> found = controller.Search();
            selectedSession = null;
            ShowSessions(found);
            ShowMessage(found.Count == 0 ? "Nothing found." : $"Records found: {found.Count}");
        }
        catch (InvalidOperationException ex)
        {
            ShowMessage(ex.Message);
        }
    }

    public void BtnClickClear(object? sender, EventArgs e)
    {
        controller.ClearCriteria();
        ResetPicker(RoomPicker);
        ResetPicker(DayPicker);
        PersonEntry.Text = "";
        selectedSession = null;
        RefreshGrid();
        ShowMessage("Search results and criteria cleared.");
    }

    public async void BtnClickAbout(object? sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AboutPage());
    }

    public async void BtnClickExit(object? sender, EventArgs e)
    {
        if (await ConfirmExit())
            Application.Current?.Quit();
    }

    public async Task<bool> ConfirmExit()
    {
        if (isExitDialogOpen)
            return false;

        isExitDialogOpen = true;
        try
        {
            ExitConfirmed = await DisplayAlertAsync("Exit",
                "Do you really want to exit the program?\nUnsaved changes will be lost.", "Yes", "No");
            return ExitConfirmed;
        }
        finally
        {
            isExitDialogOpen = false;
        }
    }

    public void ShowMessage(string text)
    {
        MessageLabel.Text = text;
    }

    private Label CreateCell(string text, Session? session)
    {
        var cell = new Label
        {
            Text = text,
            Padding = new Thickness(8, 6),
            LineBreakMode = LineBreakMode.WordWrap,
            BackgroundColor = session == null ? HeaderColor : RowColor,
            FontAttributes = session == null ? FontAttributes.Bold : FontAttributes.None,
            BindingContext = session
        };

        if (session != null)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => SelectSession(session);
            cell.GestureRecognizers.Add(tap);
        }

        return cell;
    }

    private void HighlightSelected()
    {
        foreach (Label cell in SessionsGrid.Children.OfType<Label>())
        {
            if (cell.BindingContext is Session session)
                cell.BackgroundColor = ReferenceEquals(session, selectedSession) ? SelectedColor : RowColor;
        }
    }

    private static string ConflictMessage(Session session) =>
        $"Not saved: room {session.RoomNumber} is already busy on {session.Day} at {session.Time}.";

    private static void FillPicker(Picker picker, List<string> values)
    {
        picker.Title = "";
        picker.ItemsSource = new List<string> { AllValues }.Concat(values).ToList();
        picker.SelectedIndex = 0;
    }

    private static void ResetPicker(Picker picker)
    {
        picker.SelectedIndex = picker.ItemsSource?.Count > 0 ? 0 : -1;
    }

    private static string GetSelectedValue(Picker picker)
    {
        return picker.SelectedIndex <= 0 ? "" : picker.SelectedItem?.ToString() ?? "";
    }
}
