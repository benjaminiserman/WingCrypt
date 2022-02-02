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
		Name = string.Join('.', path.Split('.')[..^1]);
	}

	public IEnumerable<string> EnumerateFiles()
	{
		yield return Path;
	}
}
