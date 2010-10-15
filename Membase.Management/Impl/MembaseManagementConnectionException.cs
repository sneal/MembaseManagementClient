using System;
using System.Runtime.Serialization;

namespace Membase.Management.Impl
{
	public class MembaseManagementConnectionException : MembaseManagementException
	{
		public MembaseManagementConnectionException()
		{
		}

		public MembaseManagementConnectionException(string message) : base(message)
		{
		}

		public MembaseManagementConnectionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected MembaseManagementConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
