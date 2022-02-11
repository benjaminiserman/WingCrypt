namespace WingCryptWPF;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using WingCryptShared;
using static FileTree;
using Path = System.IO.Path;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	const string DEFAULT_FILE_NAME = @"Selecting current directory";

	private readonly FileTree _fileTree;

	public MainWindow()
	{
		DataContext = this;
		InitializeComponent();

		_fileTree = new(fileTreeView);

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
				AddFile(fileName);
			}
		}
	}

	private void AddFile(string fileName)
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

	private bool DoEncrypt()
	{
		if (fileTreeView.Items.Count == 0)
		{
			MessageBox.Show($"No files are set to encrypt.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (!ConfirmPassword()) return false;

		Mouse.OverrideCursor = Cursors.Wait;

		string headerPath = ((TreeViewItem)fileTreeView.Items[0]).Header.ToString();
		string path = headerPath;

		string[] split = headerPath.Split('\\');
		if (split.Length > 1) path = Path.Combine(split[..^1]);

		Encryptor.Encrypt(_fileTree, path, passwordTextBox.Password);

		return true;
	}

	private void ButtonEncrypt_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			if (!DoEncrypt()) return;
		}
		catch (FileNotFoundException ex)
		{
			MessageBox.Show($"Could not find file {ex.FileName}. Did you make changes to the directory after adding it? Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		finally
		{
			Mouse.OverrideCursor = null;
		}

		fileTreeView.Items.Clear();

		MessageBox.Show("Encryption completed.", "Encryption Complete", MessageBoxButton.OK, MessageBoxImage.None);
	}

	private int DoDecrypt()
	{
		if (fileTreeView.Items.Count == 0)
		{
			MessageBox.Show($"No files are set to decypt.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return 0;
		}

		Mouse.OverrideCursor = Cursors.Wait;

		int count = 0;

		foreach (string enc in _fileTree.EnumerateFiles())
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
					throw new Exception($"Could not find file {enc}.");
				}
				catch (ZipException)
				{
					MessageBox.Show($"Cannot decrypt {enc} because file or directory {enc[..^SharedConstants.FILETYPE.Length]} already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					throw new Exception($"Cannot decrypt {enc} because file or directory {enc[..^SharedConstants.FILETYPE.Length]} already exists.");
				}
				catch (CryptographicException)
				{
					MessageBox.Show($"Decryption failed on {enc}. Your password may be incorrect.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					throw new Exception($"Decryption failed on {enc}. Your password may be incorrect.");
				}
			}
		}

		return count;
	}

	private void ButtonDecrypt_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			int count = DoDecrypt();

			if (count == 0)
			{
				MessageBox.Show($"No {SharedConstants.FILETYPE} files found to decrypt.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				fileTreeView.Items.Clear();
				MessageBox.Show("Decryption completed.", "Decryption Complete", MessageBoxButton.OK, MessageBoxImage.None);
			}
		}
		catch { }
		finally
		{
			Mouse.OverrideCursor = null;
		}
	}

	private void ButtonRemove_Click(object sender, RoutedEventArgs e)
	{
		if (fileTreeView.SelectedItem is not TreeViewItem item)
		{
			MessageBox.Show($"No file has been selected to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
			string oldPath = _fileTree.GetPath(item);

			string name = Interaction.InputBox($"What would you like to rename {item.Header} to?", "Rename Prompt", (string)item.Header);
			item.Header = name;

			string path = _fileTree.GetPath(item);

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
				if (_fileTree.EnumerateFiles().All(x => x.Length > SharedConstants.FILETYPE.Length && x[^SharedConstants.FILETYPE.Length..] == SharedConstants.FILETYPE))
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

	private void ButtonEncryptAndDelete_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			if (!DoEncrypt()) return;

			if (MessageBox.Show($"Are you sure you want to delete the unencrypted files? This is irreversible.", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				foreach (TreeViewItem item in fileTreeView.Items)
				{
					DeleteFolders(fileTreeView.Items, item.Header.ToString());
				}
			}

			MessageBox.Show("Encryption completed.", "Encryption Complete", MessageBoxButton.OK, MessageBoxImage.None);
		}
		catch (FileNotFoundException ex)
		{
			MessageBox.Show($"Could not find file {ex.FileName}. Did you make changes to the directory after adding it? Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		finally
		{
			Mouse.OverrideCursor = null;
		}

		fileTreeView.Items.Clear();
	}

	private void ButtonDecryptAndDelete_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			int count = DoDecrypt();

			if (count == 0)
			{
				MessageBox.Show($"No {SharedConstants.FILETYPE} files found to decrypt.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				if (MessageBox.Show($"Are you sure you want to delete the encrypted file(s)? This is irreversible.", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					foreach (string path in _fileTree.EnumerateFiles())
					{
						File.Delete(path);
					}
				}

				fileTreeView.Items.Clear();

				MessageBox.Show("Decryption completed.", "Decryption Complete", MessageBoxButton.OK, MessageBoxImage.None);
			}
		}
		catch { }
		finally
		{
			Mouse.OverrideCursor = null;
		}
	}

	private bool ConfirmPassword()
	{
		try
		{
			string confirm = PasswordDialog.GetPassword();

			if (confirm == passwordTextBox.Password)
			{
				return true;
			}
			else
			{
				MessageBox.Show($"Passwords did not match. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}
		catch
		{
			return false;
		}
	}

	private void FileTreeView_Drop(object send, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			foreach (string x in files)
			{
				AddFile(x);
			}
		}
	}
}