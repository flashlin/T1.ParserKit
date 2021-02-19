namespace T1.ParserKit.Core
{
	public interface IParseResult
	{
		ITextSpan[] Result { get; set; }
		ParseError Error { get; set; }
		IInputReader Rest { get; set; }
		bool IsSuccess();
	}
}