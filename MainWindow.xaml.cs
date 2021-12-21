namespace WingCrypt;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using static FileExplorer;
using Path = System.IO.Path;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	const string DEFAULT_FILE_NAME = @"Selecting current directory";

	public MainWindow()
	{
		DataContext = this;
		InitializeComponent();
	}

	private void ButtonAddFile_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog dialog = new();
		dialog.ValidateNames = false;
		dialog.CheckFileExists = false;
		dialog.CheckPathExists = true;
		dialog.FileName = DEFAULT_FILE_NAME;

		if (dialog.ShowDialog() == true)
		{
			if (dialog.FileName[^DEFAULT_FILE_NAME.Length..] == DEFAULT_FILE_NAME)
			{
				dialog.FileName = dialog.FileName[..^(DEFAULT_FILE_NAME.Length + 1)];
			}

			bool isFile = File.Exists(dialog.FileName);
			bool isDirectory = Directory.Exists(dialog.FileName);

			if (!isFile && !isDirectory)
			{
				MessageBox.Show($"File or directory {dialog.FileName} does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if ((isDirectory && ContainsAll(fileTreeView.Items, dialog.FileName)) || 
				(isFile && ContainsNode(fileTreeView.Items, dialog.FileName)))
			{
				MessageBox.Show($"Files to Encrypt/Decrypt already contains {dialog.FileName}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			int matches = 0;
			AddNodeRecursive(GetNode(fileTreeView.Items, dialog.FileName, ref matches), dialog.FileName, true, matches);
		}
	}

	private void ButtonEncrypt_Click(object sender, RoutedEventArgs e)
	{

	}

	private void ButtonDecrypt_Click(object sender, RoutedEventArgs e)
	{

	}
}
