using Microsoft.Maui.LifecycleEvents;

namespace ScheduleApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if WINDOWS
		// Ask for confirmation when the window is closed with the X button.
		builder.ConfigureLifecycleEvents(events =>
		{
			events.AddWindows(windows => windows.OnWindowCreated(window =>
			{
				window.AppWindow.Closing += async (appWindow, args) =>
				{
					if (MainPage.ExitConfirmed)
						return;

					args.Cancel = true;
					if (Shell.Current?.CurrentPage is MainPage page && await page.ConfirmExit())
						Application.Current?.Quit();
				};
			}));
		});
#endif

		return builder.Build();
	}
}
