using System;
using System.Runtime.Serialization;

namespace Membase.Management
{
	/// <summary>
	/// Base membase management exeption type.
	/// </summary>
	public class MembaseManagementException : Exception
	{
		public MembaseManagementException()
		{
		}

		public MembaseManagementException(string message) : base(message)
		{
		}

		public MembaseManagementException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected MembaseManagementException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
