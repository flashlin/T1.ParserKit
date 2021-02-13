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
			ValueHelper.CopyData(from, to);
			init(to);
			return to;
		}
	}
}
