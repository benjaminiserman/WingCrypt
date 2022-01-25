namespace WingCryptXamarin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void AddButtonClicked(object sender, EventArgs e)
	{
		//IEnumerable<FileResult> result = await FilePicker.PickMultipleAsync();
		//explainLabel.Text = $"Changed! {string.Join(", ", from x in result select x.FileName)}";
		await Navigation.PushAsync(new FolderListPage());
	}
}
