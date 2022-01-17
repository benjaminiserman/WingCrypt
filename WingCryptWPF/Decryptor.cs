namespace WingCryptWPF;
using System.IO;
using System.Security.Cryptography;
using Ionic.Zip;

internal static class Decryptor
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "parallel structure")]
	public static void Decrypt(string path, string newPath, string password)
	{
		string zipPath = Path.Combine(Path.Combine(path.Split('\\')[..^1]), SharedConstants.WORKING_NAME);

		try
		{
			using (Aes aes = Aes.Create())
			{
				using FileStream encStream = new(path, FileMode.Open);

				byte[] salt = new byte[16];
				encStream.Read(salt, 0, salt.Length);

				(byte[] keyBuffer, byte[] IV) = SharedConstants.BuildKeyAndIV(password, SharedConstants.XOR(salt));

				aes.Mode = CipherMode.CBC;
				aes.Key = keyBuffer;
				aes.IV = IV;
				aes.Padding = PaddingMode.PKCS7;

				var encryptor = aes.CreateDecryptor();

				using FileStream zipStream = new(zipPath, FileMode.Create);
				using CryptoStream cryptoStream = new(zipStream, encryptor, CryptoStreamMode.Write);

				encStream.CopyTo(cryptoStream);
			}

			using (ZipFile zip = new(zipPath))
			{
				Directory.CreateDirectory(newPath);
				zip.ExtractAll(newPath);
			}
		}
		finally
		{
			File.Delete(zipPath);
		}
	}
}