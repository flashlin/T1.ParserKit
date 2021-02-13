namespace T1.ParserKit.Core
{
	public interface IParseResult
	{
		ITextSpan[] Result { get; set; }
		ParseError Error { get; set; }
		ITextSpan Rest { get; set; }
		bool IsSuccess();
	}
}