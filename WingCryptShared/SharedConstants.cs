namespace WingCryptShared;
using System.Linq;
using System.Security.Cryptography;

public static class SharedConstants
{
	public const string WORKING_NAME = "_wingcrypttemp.zip";
	public const string FILETYPE = ".wenc";

	private static byte[] XORSalt => new byte[] { 69, 42, 39, 0, 3, 13, 87, 27, 121, 62, 1, 111, 245, 73, 199, 154 };
	// salt bytes courtesy of Kraber Queen

	public static (byte[], byte[]) BuildKeyAndIV(string key, byte[] salt)
	{
		Rfc2898DeriveBytes pbkdf2 = new(key, salt)
		{
			IterationCount = 1_024_000
		};

		return (pbkdf2.GetBytes(32), pbkdf2.GetBytes(16));
	}

	public static byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(16);

	public static byte[] XOR(byte[] salt) => salt.Zip(XORSalt, (x, y) => (byte)(x ^ y)).ToArray();
}