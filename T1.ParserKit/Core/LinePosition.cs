namespace T1.ParserKit.Core
{
	public class LinePosition
	{
		public int Line { get; set; }
		public int Col { get; set; }

		public override string ToString()
		{
			return $"Ln:{Line} Ch:{Col}";
		}
	}
}