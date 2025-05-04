using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CelOrdApp.Controllers;

public static class Extensions
{
	public static string GenerateSlug(this string phrase)
	{
		string str = phrase.RemoveDiacritics().ToLower();

		// invalid chars
		str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

		// convert multiple spaces into one space
		str = Regex.Replace(str, @"\s+", " ").Trim();

		// cut and trim
		str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
		str = Regex.Replace(str, @"\s", "-"); // hyphens

		return str;
	}

	public static string RemoveDiacritics(this string text)
	{
		var normalizedText = text.Normalize(System.Text.NormalizationForm.FormD);
		var sb = new StringBuilder(capacity: normalizedText.Length);

		for (int i = 0; i < normalizedText.Length; i++)
		{
			char c = normalizedText[i];
			var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			{
				sb.Append(c);
			}
		}

		return sb.ToString().Normalize(NormalizationForm.FormC);
	}
}
