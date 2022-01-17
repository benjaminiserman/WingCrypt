namespace WingCryptWPF;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ionic.Zip;
using Microsoft.VisualBasic;
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

		if (!string.IsNullOrWhiteSpace(App.StartPath))
		{
			if (File.Exists(App.StartPath) || Directory.Exists(App.StartPath))
			{
				int matches = 0;
				AddNodeRecursive(GetNode(fileTreeView.Items, App.StartPath, ref matches), App.StartPath, true, matches);
			}
			else
			{
				MessageBox.Show($"File or directory {App.StartPath} does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
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
				if (working.Length >= DEFAULT_FILE_NAME.Length && working[^DEFAULT_FILE_NAME.Length..] == DEFAULT_FILE_NAME) // for selecting directories
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
		if (fileTreeView.Items.Count == 0)
		{
			MessageBox.Show($"No files are set to encrypt.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		Mouse.OverrideCursor = Cursors.Wait;

		string headerPath = ((TreeViewItem)fileTreeView.Items[0]).Header.ToString();
		string path = headerPath;

		string[] split = headerPath.Split('\\');
		if (split.Length > 1) path = Path.Combine(split[..^1]);

		try
		{
			Encryptor.Encrypt(fileTreeView, path, passwordTextBox.Password);
		}
		catch (FileNotFoundException) { return; }
		finally
		{
			fileTreeView.Items.Clear();
			Mouse.OverrideCursor = null;
		}

		MessageBox.Show("Encryption completed.", "Encryption Complete", MessageBoxButton.OK, MessageBoxImage.None);
	}

	private void ButtonDecrypt_Click(object sender, RoutedEventArgs e)
	{
		if (fileTreeView.Items.Count == 0)
		{
			MessageBox.Show($"No files are set to decypt.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		Mouse.OverrideCursor = Cursors.Wait;

		int count = 0;
		bool errored = false;

		foreach (string enc in EnumerateFiles(fileTreeView))
		{
			if (enc.Length > SharedConstants.FILETYPE.Length && enc[^SharedConstants.FILETYPE.Length..] == SharedConstants.FILETYPE)
			{
				try
				{
					Decryptor.Decrypt(enc, enc[..^SharedConstants.FILETYPE.Length], passwordTextBox.Password);
					count++;
				}
				catch (IOException)
				{
					MessageBox.Show($"Could not find file {enc}. Did you make changes to the directory after adding it? Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					fileTreeView.Items.Clear();
					errored = true;
					break;
				}
				catch (ZipException)
				{
					MessageBox.Show($"Cannot decrypt {enc} because file or directory {enc[..^SharedConstants.FILETYPE.Length]} already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					errored = true;
				}
				catch (CryptographicException)
				{
					MessageBox.Show($"Decryption failed on {enc}. Your password may be incorrect.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					errored = true;
				}
			}
		}

		if (count == 0)
		{
			if (!errored) MessageBox.Show($"No {SharedConstants.FILETYPE} files found to decrypt.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		else
		{
			fileTreeView.Items.Clear();
			MessageBox.Show("Decryption completed.", "Decryption Complete", MessageBoxButton.OK, MessageBoxImage.None);
		}

		Mouse.OverrideCursor = null;
	}

	private void ButtonDelete_Click(object sender, RoutedEventArgs e)
	{
		if (fileTreeView.SelectedItem is not TreeViewItem item)
		{
			MessageBox.Show($"No file has been selected to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

	private void ButtonRename_Click(object sender, RoutedEventArgs e)
	{
		if (fileTreeView.SelectedItem is not TreeViewItem item)
		{
			MessageBox.Show($"No file has been selected to rename.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}
		else
		{
			string oldPath = GetPath(fileTreeView, item);

			string name = Interaction.InputBox($"What would you like to rename {item.Header} to?", "Rename Prompt", (string)item.Header);
			item.Header = name;

			string path = GetPath(fileTreeView, item);

			if (File.Exists(oldPath))
			{
				File.Move(oldPath, path);
			}
			else if (Directory.Exists(oldPath))
			{
				Directory.Move(oldPath, path);
			}
		}
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			if (fileTreeView.Items.Count > 0)
			{
				if (EnumerateFiles(fileTreeView).All(x => x.Length > SharedConstants.FILETYPE.Length && x[^SharedConstants.FILETYPE.Length..] == SharedConstants.FILETYPE))
				{
					ButtonDecrypt_Click(this, null);
				}
				else
				{
					ButtonEncrypt_Click(this, null);
				}
			}
		}
	}
}