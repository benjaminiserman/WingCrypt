namespace WingCryptCommand;
using CommandLine;

[Verb("encrypt", HelpText = "Encrypts the provided path.")]
internal class EncryptOptions
{
	[Option('p', "path", Required = true, HelpText = "The path to be encrypted.")]
	public string Path { get; set; }

	[Option('k', "key", Default = null, HelpText = "The password (not technically a key) to be used in encryption.")]
	public string Password { get; set; }

	[Option('d', "delete", Default = false, HelpText = "Delete path after encryption is complete.")]
	public bool Delete { get; set; }
}

[Verb("decrypt", HelpText = "Decrypt the provided path.")]
internal class DecryptOptions
{
	[Option('p', "path", Required = true, HelpText = "The path to be decrypted.")]
	public string Path { get; set; }

	[Option('k', "key", Default = null, HelpText = "The password (not technically a key) to be used in decryption.")]
	public string Password { get; set; }

	[Option('d', "delete", Default = false, HelpText = "Delete path after decryption is complete.")]
	public bool Delete { get; set; }
}

[Verb("do", isDefault: true, Hidden = false, HelpText = "Decrypts a .wenc file, or encrypts anything else.")]
internal class DoOptions
{
	[Option('p', "path", Required = true, HelpText = "The path to be encrypted/decrypted.")]
	public string Path { get; set; }

	[Option('k', "key", Default = null, HelpText = "The password (not technically a key) to be used in encryption/decryption.")]
	public string Password { get; set; }

	[Option('d', "delete", Default = false, HelpText = "Delete path after encryption/decryption is complete.")]
	public bool Delete { get; set; }
}

internal class Options
{
	public string Path { get; private set; }
	public string Password { get; set; }
	public bool Delete { get; private set; }

	public Options(EncryptOptions e)
	{
		Path = e.Path;
		Password = e.Password;
		Delete = e.Delete;
	}

	public Options(DecryptOptions e)
	{
		Path = e.Path;
		Password = e.Password;
		Delete = e.Delete;
	}

	public Options(DoOptions e)
	{
		Path = e.Path;
		Password = e.Password;
		Delete = e.Delete;
	}
}
