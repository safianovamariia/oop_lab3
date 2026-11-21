namespace ScheduleApp;

public partial class AboutPage : ContentPage
{
    private readonly string authorName = "Mariia Safianova";
    private readonly int course = 2;
    private readonly string group = "K-25";
    private readonly int year = 2026;
    private readonly string description =
        "Laboratory work No. 3, variant 8: \"Schedule of the faculty display classes and department ARMs\". " +
        "The program opens a JSON file, shows the schedule in a table, lets you add, edit and delete records " +
        "(lessons, free access time, ARM work), searches with LINQ by room, day of week and teacher / user, " +
        "and saves the data back to the JSON file.";

    public AboutPage()
    {
        InitializeComponent();
        ShowInfo();
    }

    public void ShowInfo()
    {
        AuthorLabel.Text = $"Author: {authorName}";
        CourseLabel.Text = $"Course: {course}";
        GroupLabel.Text = $"Group: {group}";
        YearLabel.Text = $"Year: {year}";
        DescriptionLabel.Text = description;
    }

    public async void BtnClickClose(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
