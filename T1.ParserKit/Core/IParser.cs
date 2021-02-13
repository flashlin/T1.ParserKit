namespace T1.ParserKit.Core
{
	public interface IParser
	{
		string Name { get; set; }
		IParseResult TryParse(ITextSpan inp);
	}
}