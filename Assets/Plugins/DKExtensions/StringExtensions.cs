public static class StringExtensions
{
	/// <summary>Deletes substring from string</summary>
	/// <param name="removeString">To delete</param>
	public static string Remove(this string sourceString, string removeString)
	{
		var index = sourceString.IndexOf(removeString);
		var cleanPath = (index < 0) ?
			sourceString :
			sourceString.Remove(index, removeString.Length);

		return cleanPath;
	}

	/// <summary>Upercase first letter of every word</summary>
	public static string UppercaseWords(this string value)
	{
		var array = value.ToCharArray();

		// Handle the first letter in the string
		if (array.Length >= 1)
		{
			if (char.IsLower(array[0]))
				array[0] = char.ToUpper(array[0]);
		}

		// Scan through the letters, checking for spaces
		// Uppercase the lowercase letters following spaces
		for (var i = 1; i < array.Length; i++)
		{
			if (array[i - 1] == ' ')
			{
				if (char.IsLower(array[i]))
					array[i] = char.ToUpper(array[i]);
			}
		}
		
		return new string(array);
	}
}