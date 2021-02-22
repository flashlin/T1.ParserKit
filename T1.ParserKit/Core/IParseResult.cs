namespace T1.ParserKit.Core
{
	public interface IParseResult<T>
	{
		T Result { get; set; }
		ParseError Error { get; set; }
		bool IsSuccess();
	}
}