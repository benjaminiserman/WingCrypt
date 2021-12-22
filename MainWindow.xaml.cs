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
		dialog.Multiselect = true;
		dialog.FileName = DEFAULT_FILE_NAME;

		if (dialog.ShowDialog() == true)
		{
			foreach (string fileName in dialog.FileNames)
			{
				string working = fileName;
				if (working.Length >= DEFAULT_FILE_NAME.Length && working[^DEFAULT_FILE_NAME.Length..] == DEFAULT_FILE_NAME)
				{
					working = working[..^(DEFAULT_FILE_NAME.Length + 1)];
				}

				bool isFile = File.Exists(working);
				bool isDirectory = Directory.Exists(working);

				if (!isFile && !isDirectory)
				{
					MessageBox.Show($"File or directory {working} does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				if ((isDirectory && ContainsAll(fileTreeView.Items, working)) ||
					(isFile && ContainsNode(fileTreeView.Items, working)))
				{
					MessageBox.Show($"Files to Encrypt/Decrypt already contains {working}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				int matches = 0;
				AddNodeRecursive(GetNode(fileTreeView.Items, working, ref matches), working, true, matches);
			}
		}
	}

	private void ButtonEncrypt_Click(object sender, RoutedEventArgs e)
	{

	}

	private void ButtonDecrypt_Click(object sender, RoutedEventArgs e)
	{

	}

	private void ButtonDelete_Click(object sender, RoutedEventArgs e)
	{
		if (fileTreeView.SelectedItem is not TreeViewItem item)
		{
			MessageBox.Show($"No files have been selected to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}
		else
		{
			if (item.Parent is TreeViewItem itemParent)
			{
				itemParent.Items.Remove(item);
			}
			else if (item.Parent is TreeView viewParent)
			{
				viewParent.Items.Remove(item);
			}
		}
	}
}
