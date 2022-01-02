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
		string zipPath = Path.Combine(path, "_wingcrypttemp.zip");
		string encPath = Path.Combine(path, "_wingcrypttemp.enc");

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
			Array.Copy(Encoding.UTF8.GetBytes(key), keyBuffer, 0);

			aes.Mode = CipherMode.CBC;
			aes.Key = keyBuffer;
			aes.IV = new byte[] { 69, 42, 39, 0, 3, 13, 87, 27, 121, 62, 1, 111, 245, 73, 199, 154 };
			// initialization vector bytes courtesy of Kraber Queen

			var encryptor = aes.CreateEncryptor();

			using FileStream encStream = new(encPath, FileMode.Create);
			using CryptoStream cryptoStream = new(encStream, encryptor, CryptoStreamMode.Write);
			using FileStream zipStream = new(zipPath, FileMode.Open);

			zipStream.CopyTo(cryptoStream);
		}
	}
}
