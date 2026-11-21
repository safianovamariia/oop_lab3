namespace ScheduleApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		UserAppTheme = AppTheme.Light;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell())
		{
			Title = "Display Class Schedule (JSON)",
			Width = 1350,
			Height = 860
		};
	}
}
