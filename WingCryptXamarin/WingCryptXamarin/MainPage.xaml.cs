namespace WingCryptXamarin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections.ObjectModel;

public partial class MainPage : ContentPage
{
	private ObservableCollection<string> Items { get; set; } = new();

	public MainPage()
	{
		InitializeComponent();

		selectedFileList.ItemsSource = Items;
	}

	private async void AddButtonClicked(object sender, EventArgs e)
	{
		//IEnumerable<FileResult> result = await FilePicker.PickMultipleAsync();
		//explainLabel.Text = $"Changed! {string.Join(", ", from x in result select x.FileName)}";
		if (!FolderListPage.Opened)
		{
			await Navigation.PushModalAsync(new FolderListPage(AddItem));
		}
	}

	private void AddItem(object sender, string s)
	{
		Items.Add(s);
		explainLabel.IsVisible = false;
	}

	private void OnSwiped(object sender, SwipedEventArgs e) => RemoveButtonClicked(sender, e); // currently unused

	private void RemoveButtonClicked(object sender, EventArgs e)
	{
		if (selectedFileList.SelectedItem is null) return;

		Items.Remove(selectedFileList.SelectedItem.ToString());

		selectedFileList.SelectedItem = null;
	}

	private void RenameButtonClicked(object sender, EventArgs e)
	{
		if (selectedFileList.SelectedItem is null) return;
	}

	private void EncryptButtonClicked(object sender, EventArgs e)
	{
		if (selectedFileList.SelectedItem is null) return;
	}

	private void DecryptButtonClicked(object sender, EventArgs e)
	{
		if (selectedFileList.SelectedItem is null) return;
	}
}
