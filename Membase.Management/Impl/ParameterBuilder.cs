using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Membase.Management.Impl
{
	/// <summary>
	/// Builds a querystring from a dictionary of key-value pairs.
	/// </summary>
	public class ParameterBuilder
	{
		/// <summary>
		/// Builds a URL Encoded querystring
		/// </summary>
		/// <remarks>
		/// if parameters is null, an empty string is returned
		/// </remarks>
		/// <param name="parameters">The key-value pairs</param>
		/// <returns>The querystring if parameters was not null</returns>
		public string DictionaryToUriEncodedString(IEnumerable<KeyValuePair<string, string>> parameters)
		{
			if (parameters == null)
			{
				return string.Empty;
			}

			var query = new StringBuilder();
			char separator = ' ';
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				query.Append(separator);
				query.Append(parameter.Key);
				query.Append("=");
				query.Append(HttpUtility.UrlEncode(parameter.Value));

				separator = '&';
			}
			return query.ToString().Trim();
		}
	}
}
