namespace WingCryptCommand;
using System.Linq;
using System.Security.Cryptography;
using CommandLine;
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
				if (options.Path.Length >= SharedConstants.FILETYPE.Length && options.Path[^SharedConstants.FILETYPE.Length..] == SharedConstants.FILETYPE)
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
			if (options.Password is null)
			{
				options.Password = GetPassword.Get(true).ToString();

				if (options.Password != GetPassword.Get(false).ToString())
				{
					Console.WriteLine("Passwords did not match.");
					return;
				}
			}

			SingleEntry entry = new(options.Path);
			Encryptor.Encrypt(entry, Path.Combine(options.Path.Split('\\')[..^1]), options.Password);

			if (options.Delete)
			{
				string path = entry.EnumerateFiles().First();

				if (Directory.Exists(path)) Directory.Delete(path, true);
				else if (File.Exists(path)) File.Delete(path);
			}

			Console.Write("Encryption complete.");
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
			if (options.Password is null)
			{
				options.Password = GetPassword.Get(true).ToString();
			}

			SingleEntry entry = new(options.Path);

			if (entry.Name[0] == '.' && entry.Name.Count(x => x == '.') == 1)
			{
				Console.WriteLine("Decrypting that file would result in a null folder, since it starts with a '.'. Rename the file and try again.");
				return;
			}

			Decryptor.Decrypt(options.Path, entry.Name, options.Password);

			Console.WriteLine("Decryption complete.");

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