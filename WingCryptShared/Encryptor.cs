namespace WingCryptShared;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Ionic.Zip;

public static class Encryptor
{
	/// <summary>
	/// Encrypts files contained within <paramref name="tree"/> and <paramref name="path"/>.
	/// </summary>
	/// <param name="tree">The file tree to be encrypted.</param>
	/// <param name="path">The path CONTAINING THE FILE TREE to be encrypted. If you're encrypting a single file, this should be the path containing the desired path.</param>
	/// <param name="password">The password to encrypt with.</param>
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
					zip.AddItem(found, Path.Combine(found.Split('\\').SubFromEnd(offset, 1)));
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

				encStream.Write(salt, 0, salt.Length);
				zipStream.CopyTo(cryptoStream);
			}
		}
		finally
		{
			try
			{
				File.Delete(zipPath);
			}
			catch { }
		}
	}
}