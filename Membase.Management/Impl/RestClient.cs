using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using log4net;

namespace Membase.Management.Impl
{
	/// <summary>
	/// Default REST client implmentation
	/// </summary>
	public class RestClient : IRestClient
	{
		private const string UserAgent = "Membase.Management";

		private static readonly ILog Log = LogManager.GetLogger(typeof (RestClient));

		private readonly Uri _managementEndpointUri;
		private readonly string _adminUserName;
		private readonly string _adminPassword;
		private readonly ParameterBuilder _parameterBuilder = new ParameterBuilder();

		public RestClient(Uri managementEndpointUri)
		{
			if (managementEndpointUri == null)
				throw new ArgumentNullException("managementEndpointUri");

			_managementEndpointUri = managementEndpointUri;
		}

		public RestClient(Uri managementEndpointUri, string adminUserName, string adminPassword)
		{
			if (managementEndpointUri == null)
				throw new ArgumentNullException("managementEndpointUri");

			_managementEndpointUri = managementEndpointUri;
			_adminUserName = adminUserName;
			_adminPassword = adminPassword;
		}

		public RestResponse Get(string resourcePath)
		{
			if (resourcePath == null)
				throw new ArgumentNullException("resourcePath");

			return ExecuteRestRequest(resourcePath, HttpMethod.Get);
		}

		public RestResponse Post(string resourcePath, IDictionary<string, string> parameters)
		{
			if (resourcePath == null)
				throw new ArgumentNullException("resourcePath");

			string formValues = _parameterBuilder.DictionaryToUriEncodedString(parameters);
			return ExecuteRestRequest(resourcePath, HttpMethod.Post, formValues);
		}

		public RestResponse Delete(string resourcePath)
		{
			if (resourcePath == null)
				throw new ArgumentNullException("resourcePath");

			return ExecuteRestRequest(resourcePath, HttpMethod.Delete);
		}

		public Uri Endpoint
		{
			get { return _managementEndpointUri; }
		}

		private RestResponse ExecuteRestRequest(string resourcePath, HttpMethod httpMethod, string formValues = null)
		{
			if (Log.IsDebugEnabled)
			{
				Log.DebugFormat("REST {0} to {1}", httpMethod, resourcePath);
			}

			HttpWebResponse webResponse;
			try
			{
				HttpWebRequest request = CreateHttpWebRequest(resourcePath, httpMethod, formValues);
				webResponse = (HttpWebResponse) request.GetResponse();
			}
			catch (WebException ex)
			{
				var exResponse = ex.Response as HttpWebResponse;
				if (exResponse != null)
				{
					Log.Debug("REST invocation returned an error code", ex);
					webResponse = exResponse;
				}
				else
				{
					Log.Error("REST invocation threw an unexpected exception", ex);
					throw new MembaseManagementConnectionException(
						"An unexpected error occurred trying to connect to the Membase server", ex);
				}
			}

			return new RestResponse(webResponse.StatusCode, GetResponseBodyAsString(webResponse));
		}

		private HttpWebRequest CreateHttpWebRequest(string resourcePath, HttpMethod httpMethod, string formValues)
		{
			var request = (HttpWebRequest)WebRequest.Create(CreateResourceUri(resourcePath));
			request.Method = httpMethod.ToString();
			request.Accept = MimeTypes.Json;
			request.UserAgent = UserAgent;

			if (!string.IsNullOrWhiteSpace(_adminUserName))
			{
				request.Credentials = new NetworkCredential(_adminUserName, _adminPassword);
			}

			if (httpMethod == HttpMethod.Post)
			{
				request.ContentType = MimeTypes.FormPost;
				SetFormPostValues(request, formValues);
			}

			return request;
		}

		private static void SetFormPostValues(HttpWebRequest request, string formValues)
		{
			var bytes = Encoding.UTF8.GetBytes(formValues);
			request.ContentLength = bytes.Length;
			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}
		}

		private static string GetResponseBodyAsString(HttpWebResponse response)
		{
			Stream responseStream = response.GetResponseStream();
			if (responseStream == null)
			{
				return string.Empty;
			}

			string body = new StreamReader(responseStream).ReadToEnd();
			return body;
		}

		private Uri CreateResourceUri(string resourcePath)
		{
			string uri = _managementEndpointUri + resourcePath;
			return new Uri(uri);
		}
	}
}
