namespace WingCryptShared;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Ionic.Zip;

public static class Encryptor
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "parallel structure")]
	public static void Encrypt(IFileTree tree, string path, string password)
	{
		string zipPath = Path.Combine(path, SharedConstants.WORKING_NAME);
		string encPath = Path.Combine(path, $"{tree.Name}{SharedConstants.FILETYPE}");

		try
		{
			using (ZipFile zip = new())
			{
				string[] pathSplit = path.Split('\\');
				int offset = pathSplit.Length;
				if (pathSplit.Length > 1) offset++;

				int count = tree.EnumerateFiles().Count(); // fix single-file encryption
				if (count == 1) offset--;

				foreach (string found in tree.EnumerateFiles())
				{
					zip.AddItem(found, Path.Combine(found.Split('\\')[offset..^1]));
				}

				zip.Save(zipPath);
			}

			using (Aes aes = Aes.Create())
			{
				byte[] salt = SharedConstants.GenerateSalt();
				(byte[] keyBuffer, byte[] IV) = SharedConstants.BuildKeyAndIV(password, SharedConstants.XOR(salt));

				aes.Mode = CipherMode.CBC;
				aes.Key = keyBuffer;
				aes.IV = IV;
				aes.Padding = PaddingMode.PKCS7;

				var encryptor = aes.CreateEncryptor();

				using FileStream encStream = new(encPath, FileMode.Create);
				using CryptoStream cryptoStream = new(encStream, encryptor, CryptoStreamMode.Write);
				using FileStream zipStream = new(zipPath, FileMode.Open);

				encStream.Write(salt);
				zipStream.CopyTo(cryptoStream);
			}
		}
		finally
		{
			File.Delete(zipPath);
		}
	}
}