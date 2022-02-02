namespace WingCryptCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

[Verb("encrypt", HelpText = "Encrypts the provided path")]
internal class EncryptOptions
{
	[Option('p', "path", Required = true, HelpText = "The path to be encrypted.")]
	public string Path { get; set; }

	[Option('k', "key", Required = true, HelpText = "The password (not technically a key) to be used in encryption.")]
	public string Password { get; set; }

	[Option('d', "delete", Default = false, HelpText = "Delete path after encryption is complete.")]
	public bool Delete { get; set; }
}

[Verb("decrypt", HelpText = "Decrypt the provided path")]
internal class DecryptOptions
{
	[Option('p', "path", Required = true, HelpText = "The path to be decrypted.")]
	public string Path { get; set; }

	[Option('k', "key", Required = true, HelpText = "The password (not technically a key) to be used in decryption.")]
	public string Password { get; set; }

	[Option('d', "delete", Default = false, HelpText = "Delete path after decryption is complete.")]
	public bool Delete { get; set; }
}
