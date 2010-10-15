using System.Collections.Generic;

namespace Membase.Management.Resources
{
	public class Bucket
	{
		public string Name { get; set; }
		public BucketRules BucketRules { get; set; }
		public List<Node> Nodes { get; set; }
	}
}
