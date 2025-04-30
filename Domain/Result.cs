namespace Domain;

public class Result<TValue> where TValue : class, new()
{
	public bool IsSuccess { get { return _isSuccess; } }
	public bool IsFailure => !IsSuccess;
	public string Error { get { return _error; } }
	public TValue Value { get { return _value; } }

	private readonly bool _isSuccess;

	private readonly TValue _value;
	private readonly string _error;

	private Result(TValue value)
	{
		_isSuccess = true;
		_value = value;
		_error = string.Empty;
	}

	private Result(string error)
	{
		_isSuccess = false;
		_value = new TValue();
		_error = error;
	}

	public static Result<TValue> Success(TValue value) => new(value);

	public static Result<TValue> Failure(string error) => new(error);
}

public record MessageObj(int Code, string Description)
{
	public static MessageObj None => new(0, string.Empty);
}
