namespace WingCryptCommand;

using CommandLine;
using WingCryptShared;

public static class Program
{
	public static void Main(string[] args)
	{
		var result = Parser.Default.ParseArguments<EncryptOptions, DecryptOptions>(args)
			.WithParsed<EncryptOptions>(options =>
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
				catch (IOException)
				{
					Console.WriteLine($"Path {options.Path} was not found.");
				}
			})
			.WithParsed<DecryptOptions>(options =>
			{
				try
				{
					SingleEntry entry = new(options.Path);
					Decryptor.Decrypt(options.Path, entry.Name, options.Password);

					if (options.Delete) File.Delete(entry.EnumerateFiles().First());
				}
				catch (IOException)
				{
					Console.WriteLine($"Path {options.Path} was not found.");
				}
			});
	}
}