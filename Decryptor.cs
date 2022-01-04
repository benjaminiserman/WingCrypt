namespace WingCrypt;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Ionic.Zip;

internal static class Decryptor
{ 
	public static void Decrypt(string path, string newPath, string key)
	{
		string zipPath = Path.Combine(Path.Combine(path.Split('\\')[..^1]), SharedConstants.WORKING_NAME);

		using (Aes aes = Aes.Create())
		{
			byte[] keyBuffer = SharedConstants.BuildKey(key);

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
