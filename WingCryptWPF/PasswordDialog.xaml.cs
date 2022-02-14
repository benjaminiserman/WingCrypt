namespace WingCryptWPF;
using System;
using System.Windows;
using System.Windows.Input;
/// <summary>
/// Interaction logic for PasswordDialog.xaml
/// </summary>
public partial class PasswordDialog : Window
{
	public PasswordDialog()
	{
		InitializeComponent();
	}

	public string Password => passwordTextBox.Password;

	public static string GetPassword()
	{
		PasswordDialog passwordDialog = new();

		if (passwordDialog.ShowDialog() == true)
		{
			return passwordDialog.Password;
		}
		else throw new Exception("Cancelled.");
	}

	private void ButtonConfirm_Click(object sender, RoutedEventArgs e) => DialogResult = true;

	private void OnKeyDownHandler(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			ButtonConfirm_Click(sender, e);
		}
	}
}
