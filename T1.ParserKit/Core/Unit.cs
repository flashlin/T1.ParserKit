namespace T1.ParserKit.Core
{
	public sealed class Unit
	{
		private Unit() { }

		public static Unit Instance { get; } = new Unit();
	}
}