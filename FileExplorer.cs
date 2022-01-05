namespace WingCrypt;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

internal static class FileExplorer
{
	internal static ItemCollection GetNode(ItemCollection collection, string path, ref int matches)
	{
		string[] pathSplit = path.Split('\\');

		foreach (TreeViewItem item in collection)
		{
			string nodePath = (string)item.Header;
			string[] nodeSplit = nodePath.Split('\\');

			int thisMatches = IntersectPaths(pathSplit, nodeSplit, matches);
			matches += thisMatches;

			if (thisMatches == nodeSplit.Length)
			{
				if (matches == pathSplit.Length)
				{
					collection.Clear();
					return collection;
				}
				else
				{
					ItemCollection node = GetNode(item.Items, path, ref matches);
					return node;
				}
			}
			else if (thisMatches != 0)
			{
				string newPath = Path.Combine(pathSplit[(matches - thisMatches)..matches]);
				TreeViewItem node = new() { Header = newPath };

				item.Header = Path.Combine(nodeSplit[thisMatches..]);

				collection.Add(node);
				collection.Remove(item);

				if (thisMatches != pathSplit.Length) node.Items.Add(item);

				return node.Items;
			}
		}

		return collection;
	}

	internal static void AddNodeRecursive(ItemCollection collection, string path, bool head, int matches)
	{
		bool isFile = File.Exists(path);
		bool isDirectory = Directory.Exists(path);

		string name = head ? Path.Combine(path.Split('\\')[matches..]) : path.Split('\\')[^1];

		if (isFile)
		{
			collection.Add(new TreeViewItem() { Header = name });
		}
		else if (isDirectory)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				TreeViewItem subdirectory = new() { Header = name };
				collection.Add(subdirectory);
				collection = subdirectory.Items;
			}

			foreach (string subPath in Directory.EnumerateFileSystemEntries(path))
			{
				AddNodeRecursive(collection, subPath, false, matches);
			}
		}
	}

	internal static int IntersectPaths(string a, string b, int start = 0)
	{
		string[] splitA = a.Split('\\');
		string[] splitB = b.Split('\\');

		return IntersectPaths(splitA, splitB, start);
	}

	internal static int IntersectPaths(string[] splitA, string[] splitB, int start = 0)
	{
		int length = splitA.Length < splitB.Length ? splitA.Length : splitB.Length;
		int matches = 0;

		for (int i = 0; i < length; i++)
		{
			if (splitA[i + start] == splitB[i])
			{
				matches++;
			}
			else break;
		}

		return matches;
	}

	internal static bool ContainsNode(ItemCollection collection, string searchPath, string workingPath = "")
	{
		foreach (TreeViewItem item in collection)
		{
			string newPath = Path.Combine(workingPath, (string)item.Header);
			if (newPath == searchPath) return true;

			if (item.Items.Count != 0 && ContainsNode(item.Items, searchPath, newPath)) return true;
		}

		return false;
	}

	private static TreeViewItem Find(ItemCollection collection, string path, int matches = 0)
	{
		string[] pathSplit = path.Split('\\');

		foreach (TreeViewItem item in collection)
		{
			string nodePath = (string)item.Header;
			string[] nodeSplit = nodePath.Split('\\');

			int count = IntersectPaths(pathSplit, nodeSplit, matches);
			matches += count;

			if (count == nodeSplit.Length)
			{
				return matches == pathSplit.Length
					? item
					: Find(item.Items, path, matches);
			}
		}

		return null;
	}

	internal static bool ContainsAll(ItemCollection collection, string path, bool find = true)
	{
		IEnumerable<string> paths = Directory.EnumerateFileSystemEntries(path);

		if (find)
		{
			TreeViewItem node = Find(collection, path);
			if (node is null) return false;
			else collection = node.Items;
		}

		int count = 0;
		foreach (TreeViewItem item in collection)
		{
			if (paths.Any(x => x.Split('\\')[^1] == (string)item.Header))
			{
				string newPath = paths.First(x => x.Split('\\')[^1] == (string)item.Header);
				if (Directory.Exists(newPath) && !ContainsAll(item.Items, Path.Combine(path, (string)item.Header), false)) return false;
				count++;
			}
		}

		return count == paths.Count();
	}

	internal static IEnumerable<string> EnumerateFiles(TreeView tree)
	{
		foreach (TreeViewItem item in tree.Items)
		{
			foreach (string found in EnumerateInner(item, string.Empty)) yield return found;
		}
	}

	private static IEnumerable<string> EnumerateInner(TreeViewItem item, string path)
	{
		path = Path.Combine(path, item.Header.ToString());

		if (item.Items.Count > 0)
		{
			foreach (TreeViewItem next in item.Items)
			{
				foreach (string found in EnumerateInner(next, path)) yield return found;
			}
		}
		else
		{
			yield return path;
		}
	}

	internal static string GetPath(TreeView tree, TreeViewItem item)
	{
		foreach (TreeViewItem found in tree.Items)
		{
			string foundPath = GetPath(found, item, (string)found.Header);

			if (!string.IsNullOrEmpty(foundPath)) return foundPath;
		}

		return null;
	}

	private static string GetPath(TreeViewItem node, TreeViewItem item, string path)
	{
		if (node == item) return path;

		foreach (TreeViewItem found in node.Items)
		{
			string foundPath = GetPath(found, item, Path.Combine(path, (string)found.Header));

			if (!string.IsNullOrEmpty(foundPath)) return foundPath;
		}

		return null;
	}
}