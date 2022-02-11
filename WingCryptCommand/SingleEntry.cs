namespace WingCryptCommand;
using System.Collections.Generic;
using WingCryptShared;

internal class SingleEntry : IFileTree
{
	public string Name { get; private set; }

	private string Path { get; set; }

	public SingleEntry(string path)
	{
		Path = path;
		string[] split = path.Split('.');
		
		Name = string.Join('.', split[..^1]);

		if (string.IsNullOrWhiteSpace(Name))
		{
			Name = path;
		}

		if (string.IsNullOrWhiteSpace(Name))
		{
			throw new ArgumentException($"Name could not be determined from path {path}. You may want to try renaming your path.");
		}

		Console.WriteLine($"{path}: {split.Length}");
		Console.WriteLine($"Name: {Name}");
	}

	public IEnumerable<string> EnumerateFiles()
	{
		yield return Path;
	}
}
