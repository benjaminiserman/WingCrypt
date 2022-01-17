namespace WingCrypt;
using System.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public static string StartPath { get; private set; } = null;

	private void ApplicationStartup(object sender, StartupEventArgs e)
	{
		if (e.Args.Length > 0) StartPath = e.Args[0];

		new MainWindow().Show();
	}
}