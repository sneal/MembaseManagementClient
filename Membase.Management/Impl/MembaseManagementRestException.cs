using System;
using System.Net;
using System.Runtime.Serialization;

namespace Membase.Management.Impl
{
	public class MembaseManagementRestException : MembaseManagementException
	{
		public HttpStatusCode StatusCode { get; set; }

		public MembaseManagementRestException(string message, HttpStatusCode statusCode) : base(message)
		{
			StatusCode = statusCode;
		}

		public MembaseManagementRestException(string message, Exception innerException, HttpStatusCode statusCode) : base(message, innerException)
		{
			StatusCode = statusCode;
		}

		protected MembaseManagementRestException(SerializationInfo info, StreamingContext context, HttpStatusCode statusCode) : base(info, context)
		{
			StatusCode = statusCode;
		}

		public MembaseManagementRestException(HttpStatusCode statusCode)
		{
			StatusCode = statusCode;
		}
	}
}
