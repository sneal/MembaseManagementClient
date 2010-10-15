using System;

namespace Membase.Management.IntegrationTests
{
	public static class TestConfiguration
	{
		public static Uri MembaseManagementUri
		{
			get { return new Uri("http://127.0.0.1:8091"); }
		}

		public static string AdminUserName
		{
			get { return "Administrator"; }
		}

		public static string AdminPassword
		{
			get { return "password"; }
		}
	}
}
