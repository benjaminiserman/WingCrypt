namespace WingCrypt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using Ionic.Zip;

internal static class Encryptor
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "parallel structure")]
	public static void Encrypt(TreeView tree, string path, string key)
	{
		string name = (string)((TreeViewItem)tree.Items[0]).Header;

		if (((TreeViewItem)tree.Items[0]).Items.Count != 0 && !FileExplorer.ContainsAll(tree.Items, name))
		{
			name = Path.Combine(name, (string)((TreeViewItem)((TreeViewItem)tree.Items[0]).Items[0]).Header);
		}

		string zipPath = Path.Combine(path, SharedConstants.WORKING_NAME);
		string encPath = Path.Combine(path, $"{name}{SharedConstants.FILETYPE}");

		try
		{
			using (ZipFile zip = new())
			{
				string[] pathSplit = path.Split('\\');
				int offset = pathSplit.Length;
				if (pathSplit.Length > 1) offset++;

				int count = FileExplorer.EnumerateFiles(tree).Count(); // fix single-file encryption
				if (count == 1) offset--;

				foreach (string found in FileExplorer.EnumerateFiles(tree))
				{
					try
					{
						zip.AddItem(found, Path.Combine(found.Split('\\')[offset..^1]));
					}
					catch
					{
						MessageBox.Show($"Could not find file {found}. Did you make changes to the directory after adding it? Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						throw;
					}
				}

				zip.Save(zipPath);
			}

			using (Aes aes = Aes.Create())
			{
				byte[] keyBuffer = SharedConstants.BuildKey(key);

				aes.Mode = CipherMode.CBC;
				aes.Key = keyBuffer;
				aes.IV = SharedConstants.IV;
				aes.Padding = PaddingMode.PKCS7;

				var encryptor = aes.CreateEncryptor();

				using FileStream encStream = new(encPath, FileMode.Create);
				using CryptoStream cryptoStream = new(encStream, encryptor, CryptoStreamMode.Write);
				using FileStream zipStream = new(zipPath, FileMode.Open);

				zipStream.CopyTo(cryptoStream);
			}
		}
		finally
		{
			File.Delete(zipPath);
		}
	}
}
