namespace WingCryptShared;
using System.Collections.Generic;

public interface IFileTree
{
	string Name { get; }

	IEnumerable<string> EnumerateFiles();
}
