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

internal static class Decryptor
{ 
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "parallel structure")]
	public static void Decrypt(string path, string newPath, string key)
	{
		string zipPath = Path.Combine(newPath, SharedConstants.WORKING_NAME);

		using (Aes aes = Aes.Create())
		{
			byte[] keyBuffer = new byte[32];
			Array.Copy(Encoding.UTF8.GetBytes(key), keyBuffer, 0);

			aes.Mode = CipherMode.CBC;
			aes.Key = keyBuffer;
			aes.IV = SharedConstants.IV;

			var encryptor = aes.CreateEncryptor();

			using FileStream zipStream = new(zipPath, FileMode.Create);
			using CryptoStream cryptoStream = new(zipStream, encryptor, CryptoStreamMode.Read);
			using FileStream encStream = new(path, FileMode.Open);

			encStream.CopyTo(cryptoStream);
		}
	}
}
