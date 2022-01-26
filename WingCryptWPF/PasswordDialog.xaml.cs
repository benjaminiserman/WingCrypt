namespace WingCryptWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
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

	private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
	{
		DialogResult = true;
	}
}
