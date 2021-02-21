namespace T1.ParserKit.Core
{
	public interface IParseResult<T>
	{
		ITextSpan TextSpan { get; set; }
		T Result { get; set; }
		ParseError Error { get; set; }
		IInputReader Rest { get; set; }
		bool IsSuccess();
	}
}