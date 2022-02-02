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
				}
				catch
				{
					throw;
				}
			})
			.WithParsed<DecryptOptions>(options =>
			{
				SingleEntry entry = new(options.Path);
				Decryptor.Decrypt(options.Path, entry.Name, options.Password);
			});
	}
}