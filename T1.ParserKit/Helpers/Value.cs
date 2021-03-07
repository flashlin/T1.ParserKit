using System;
using System.Collections.Generic;
using System.Text;
using T1.Standard.Common;

namespace T1.ParserKit.Helpers
{
	public static class Value
	{
		public static T2 Assign<T2>(object from, Action<T2> init)
			where T2: class, new()
		{
			var to = new T2();
			ValueHelper.CopyData(@from, to);
			init(to);
			return to;
		}

		public static string GetCStyleStringText(this string cstyleString)
		{
			if (string.IsNullOrEmpty(cstyleString))
			{
				return cstyleString;
			}

			//var start = cstyleString.Substring(0, 1);
			//var end = cstyleString.Substring(cstyleString.Length - 1, 1);
			//if (start == end && start == "\"")
			//{

			//}
			return cstyleString.Substring(1, cstyleString.Length - 2);
		}

		public static bool IsToggle(this string onOffText)
		{
			return string.Equals(onOffText.ToUpper(), "ON", StringComparison.Ordinal);
		}
	}
}
