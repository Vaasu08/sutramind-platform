using System.Configuration;
using System.Data;
using System.IO;
using System.Windows.Threading;
using System.Windows;

namespace SutraMind.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
		DispatcherUnhandledException += HandleDispatcherUnhandledException;
		AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
		ShowLogin();
	}

	public void ShowLogin(MainWindow? currentWindow = null)
	{
		try
		{
			currentWindow?.Hide();
			var login = new LoginWindow();
			var authenticated = login.ShowDialog() == true;
			if (!authenticated)
			{
				currentWindow?.Close();
				Shutdown();
				return;
			}

			var mainWindow = currentWindow ?? new MainWindow();
			mainWindow.SetAuthenticatedUser(login.AuthenticatedEmail);
			MainWindow = mainWindow;
			mainWindow.Show();
			mainWindow.Activate();
		}
		catch (Exception exception)
		{
			ReportFatalError("The clinical workspace could not be opened.", exception);
		}
	}

	private void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		e.Handled = true;
		ReportFatalError("The desktop workspace encountered an unexpected error.", e.Exception);
	}

	private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		if (e.ExceptionObject is Exception exception)
			WriteErrorLog(exception);
	}

	private void ReportFatalError(string message, Exception exception)
	{
		WriteErrorLog(exception);
		MessageBox.Show($"{message}\n\nA diagnostic entry was saved locally.\n\n{exception.Message}", "SutraMind error", MessageBoxButton.OK, MessageBoxImage.Error);
		Shutdown(1);
	}

	private static void WriteErrorLog(Exception exception)
	{
		try
		{
			var logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SutraMind", "logs");
			Directory.CreateDirectory(logDirectory);
			var logPath = Path.Combine(logDirectory, "desktop-errors.log");
			File.AppendAllText(logPath, $"[{DateTimeOffset.UtcNow:O}] {exception}\n\n");
		}
		catch
		{
			// Error reporting must never cause another application failure.
		}
	}
}

