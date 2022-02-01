namespace WingCryptXamarin;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WingCryptShared;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class FolderListPage : ContentPage
{
	public ObservableCollection<string> Items { get; set; }

	private event Action<object, string> OnFinish;

	private string _currentPath = "/sdcard/";

	public static bool Opened { get; private set; }

	public FolderListPage(Action<object, string> callback)
	{
		Opened = true;

		InitializeComponent();

		OnFinish += callback;

		Load();
	}

	void Choose(string s) => _currentPath = $"{Path.Combine(_currentPath, s.Trim('/'))}/";

	void Back() => _currentPath = $"/{Path.Combine(_currentPath.Split('/').SubFromEnd(0, 2))}/";

	void Load()
	{
		var directories = Directory.EnumerateDirectories(_currentPath);

		directories = from x in directories select x.Substring(_currentPath.Length);

		Items = new ObservableCollection<string>(directories);

		fileList.ItemsSource = Items;
	}

	protected override bool OnBackButtonPressed()
	{
		if (_currentPath == "/sdcard/")
		{
			Opened = false;
			return base.OnBackButtonPressed();
		}
		else
		{
			Back();
			Load();

			return true;
		}
	}

	private async void SelectButtonClicked(object sender, EventArgs e)
	{
		if (fileList.SelectedItem is null) return;

		Choose(fileList.SelectedItem.ToString());
		OnFinish(this, _currentPath);

		Opened = false;

		await Navigation.PopModalAsync();	
	}

	private void OpenButtonClicked(object sender, EventArgs e)
	{
		if (fileList.SelectedItem is null) return;

		Choose(fileList.SelectedItem.ToString());
		Load();

		fileList.SelectedItem = null;
	}
}
