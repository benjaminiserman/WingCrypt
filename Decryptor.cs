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
		string zipPath = Path.Combine(Path.Combine(path.Split('\\')[..^1]), SharedConstants.WORKING_NAME);

		using (Aes aes = Aes.Create())
		{
			byte[] keyBuffer = new byte[32];
			byte[] encoded = Encoding.UTF8.GetBytes(key);
			Array.Copy(encoded, keyBuffer, keyBuffer.Length < encoded.Length ? keyBuffer.Length : encoded.Length);

			File.WriteAllBytes("testdump.txt", keyBuffer);

			aes.Mode = CipherMode.CBC;
			aes.Key = keyBuffer;
			aes.IV = SharedConstants.IV;
			aes.Padding = PaddingMode.PKCS7;

			var encryptor = aes.CreateDecryptor();

			using FileStream zipStream = new(zipPath, FileMode.Create);
			using CryptoStream cryptoStream = new(zipStream, encryptor, CryptoStreamMode.Write);
			using FileStream encStream = new(path, FileMode.Open);

			encStream.CopyTo(cryptoStream);
		}

		using (ZipFile zip = new(zipPath))
		{
			Directory.CreateDirectory(newPath);
			zip.ExtractAll(newPath);
		}

		File.Delete(zipPath);
	}
}
