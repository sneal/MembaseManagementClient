using System;

namespace Membase.Management.Resources
{
	public class Node
	{
		public string HostName { get; set; }
		public Uri Uri { get; set; }
		public string Status { get; set; }
		public Ports Ports { get; set; }
	}
}
