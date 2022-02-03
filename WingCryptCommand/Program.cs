namespace WingCryptCommand;

using CommandLine;
using System.Security.Cryptography;
using WingCryptShared;

public static class Program
{
	public static void Main(string[] args)
	{
		var result = Parser.Default.ParseArguments<EncryptOptions, DecryptOptions, DoOptions>(args)
			.WithParsed<EncryptOptions>(options => Encrypt(new(options)))
			.WithParsed<DecryptOptions>(options => Decrypt(new(options)))
			.WithParsed<DoOptions>(options =>
			{
				if (options.Path.Length > SharedConstants.FILETYPE.Length && options.Path[^SharedConstants.FILETYPE.Length..] == SharedConstants.FILETYPE)
				{
					Decrypt(new(options));
				}
				else
				{
					Encrypt(new(options));
				}
			});
	}

	private static void Encrypt(Options options)
	{
		try
		{
			SingleEntry entry = new(options.Path);
			Encryptor.Encrypt(entry, Path.Combine(options.Path.Split('\\')[..^1]), options.Password);

			if (options.Delete)
			{
				string path = entry.EnumerateFiles().First();

				if (Directory.Exists(path)) Directory.Delete(path, true);
				else if (File.Exists(path)) File.Delete(path);
			}
		}
		catch (IOException e)
		{
			Console.WriteLine(e.Message);
		}
	}

	private static void Decrypt(Options options)
	{
		try
		{
			SingleEntry entry = new(options.Path);
			Decryptor.Decrypt(options.Path, entry.Name, options.Password);

			if (options.Delete) File.Delete(entry.EnumerateFiles().First());
		}
		catch (IOException e)
		{
			Console.WriteLine(e.Message);
		}
		catch (ZipException)
		{
			Console.WriteLine($"Cannot decrypt {options.Path} because file or directory {options.Path[..^SharedConstants.FILETYPE.Length]} already exists.");
		}
		catch (CryptographicException)
		{
			Console.WriteLine($"Decryption failed. Your password may be incorrect.");
		}
	}
}