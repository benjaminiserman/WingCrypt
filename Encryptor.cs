namespace WingCrypt;
using System.IO;
using System.Security.Cryptography;
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

				foreach (string found in FileExplorer.EnumerateFiles(tree))
				{
					zip.AddItem(found, Path.Combine(found.Split('\\')[offset..^1]));
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
		catch
		{
			File.Delete(zipPath);
			throw;
		}

		File.Delete(zipPath);
	}
}
