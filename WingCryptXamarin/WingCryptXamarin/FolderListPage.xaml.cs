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

	private string _currentPath = "/sdcard/";

	public FolderListPage()
	{
		InitializeComponent();

		Load();
	}

	void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
	{
		if (e.Item == null) return;

		Choose(((ListView)sender).SelectedItem.ToString());
	}

	void Choose(string s)
	{
		_currentPath = Path.Combine(_currentPath, s);
		Load();
	}

	void Back()
	{
		_currentPath = Path.Combine(_currentPath.Split().SubFromEnd(0, 1));
		Load();
	}

	void Load()
	{
		var directories = Directory.EnumerateDirectories(_currentPath);

		directories = from x in directories select x.Substring(_currentPath.Length);

		Items = new ObservableCollection<string>(directories);

		MyListView.ItemsSource = Items;
	}

	private void BackButtonClicked(object sender, EventArgs e) => Back();
}
