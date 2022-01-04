namespace WingCrypt;
using System.Text;

internal static class SharedConstants
{
	internal const string WORKING_NAME = "_wingcrypttemp.zip";
	internal const string FILETYPE = ".enc";

	internal static byte[] IV => new byte[] { 69, 42, 39, 0, 3, 13, 87, 27, 121, 62, 1, 111, 245, 73, 199, 154 };
	// initialization vector bytes courtesy of Kraber Queen

	public static byte[] BuildKey(string key)
	{
		byte[] keyBuffer = new byte[32];
		byte[] encoded = Encoding.UTF8.GetBytes(key);

		for (int i = 0; i < encoded.Length; i++)
		{
			keyBuffer[i % keyBuffer.Length] ^= encoded[i];
		}

		return keyBuffer;
	}
}
