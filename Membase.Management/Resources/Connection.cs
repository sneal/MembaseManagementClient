using System.Collections.Generic;

namespace Membase.Management.Resources
{
	public class Connection
	{
		public string ImplementationVersion { get; set; }
		public List<Pool> Pools { get; set; }
	}
}
