using System;
using System.Text.RegularExpressions;

namespace PhiClient
{
	// Token: 0x0200001D RID: 29
	public static class TextHelper
	{
		// Token: 0x0600004A RID: 74 RVA: 0x00003A1C File Offset: 0x00001C1C
		public static string StripRichText(string input, params string[] strippedTags)
		{
			foreach (string str in strippedTags)
			{
				input = new Regex("<\\/?" + str + "(=[\\w#]+)?>").Replace(input, "");
			}
			return input;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003A60 File Offset: 0x00001C60
		public static string StripRichText(string input)
		{
			return TextHelper.StripRichText(input, new string[]
			{
				"size",
				"b",
				"i",
				"color"
			});
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003A90 File Offset: 0x00001C90
		public static string Clamp(string input, int min, int max, char filler = '-')
		{
			int length = TextHelper.StripRichText(input).Length;
			if (length < min)
			{
				input += new string(filler, min);
			}
			else if (length > max)
			{
				input = input.Substring(0, max);
			}
			return input;
		}

		// Token: 0x0400005F RID: 95
		public const string SIZE = "size";

		// Token: 0x04000060 RID: 96
		public const string B = "b";

		// Token: 0x04000061 RID: 97
		public const string I = "i";

		// Token: 0x04000062 RID: 98
		public const string COLOR = "color";
	}
}
