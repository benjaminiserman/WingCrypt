namespace WingCrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;
using Ionic.Zip;

internal static class Encryptor
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "parallel structure")]
	public static void Encrypt(TreeView tree, string path, string key)
	{
		string zipPath = Path.Combine(path, SharedConstants.WORKING_NAME);
		string encPath = Path.Combine(path, $"{((TreeViewItem)tree.Items[0]).Header}{SharedConstants.FILETYPE}");

		using (ZipFile zip = new())
		{

			string[] pathSplit = path.Split('\\');
			int offset = pathSplit.Length;
			if (pathSplit.Length > 1) offset++;

			foreach (string found in FileExplorer.EnumerateFiles(tree))
			{
				zip.AddItem(found, Path.Combine(found.Split('\\')[offset..]));
			}

			zip.Save(zipPath);
		}

		using (Aes aes = Aes.Create())
		{
			byte[] keyBuffer = new byte[32];
			byte[] encoded = Encoding.UTF8.GetBytes(key);
			Array.Copy(encoded, keyBuffer, keyBuffer.Length < encoded.Length ? keyBuffer.Length : encoded.Length);

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

		File.Delete(zipPath);
	}
}
