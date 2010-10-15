using System.Net;

namespace Membase.Management.Impl
{
	/// <summary>
	/// The result of a REST call to the Membase management server.
	/// </summary>
	public class RestResponse
	{
		public RestResponse(HttpStatusCode statusCode, string body)
		{
			StatusCode = statusCode;
			Body = body;
		}

		/// <summary>
		/// The HTTP status code of the response.
		/// </summary>
		public HttpStatusCode StatusCode { get; private set; }

		/// <summary>
		/// The response body.
		/// </summary>
		/// <remarks>
		/// This could be JSON or plain text.
		/// </remarks>
		public string Body { get; private set; }
		
		/// <summary>
		/// Returns true if the StatusCode is a negative (400 or higher) response.
		/// </summary>
		public bool IsError
		{
			get
			{
				int status = (int)StatusCode;
				return status >= 400;
			}
		}

	}
}
