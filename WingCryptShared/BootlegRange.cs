namespace WingCryptShared;
public static class BootlegRange
{
	// [x..y]
	public static T[] Sub<T>(this T[] array, int start, int end)
	{
		T[] output = new T[end - start];

		for (int i = start; i < end; i++)
		{
			output[i - start] = array[i];
		}

		return output;
	}

	public static T[] Sub<T>(this T[] array, int start) => Sub(array, start, array.Length);

	// [x..^y]
	public static T[] SubFromEnd<T>(this T[] array, int start, int end) => Sub(array, start, array.Length - end);
}
