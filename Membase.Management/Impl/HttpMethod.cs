namespace Membase.Management.Impl
{
	/// <summary>
	/// Type safe enum for HTTP methods
	/// </summary>
	public class HttpMethod
	{
		public static readonly HttpMethod Get = new HttpMethod("GET");
		public static readonly HttpMethod Post = new HttpMethod("POST");
		public static readonly HttpMethod Delete = new HttpMethod("DELETE");

		private readonly string _httpMethod;

		private HttpMethod(string httpMethod)
		{
			_httpMethod = httpMethod;
		}

		public override string ToString()
		{
			return _httpMethod;
		}
	}
}
