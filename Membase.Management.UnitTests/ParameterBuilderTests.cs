using System.Collections.Generic;
using Membase.Management.Impl;
using NUnit.Framework;

namespace Membase.Management.UnitTests.Impl
{
	[TestFixture]
	public class ParameterBuilderTests
	{
		readonly ParameterBuilder _builder = new ParameterBuilder();

		[Test]
		public void DictionaryToUriEncodedString_returns_empty_string_when_passed_a_null()
		{
			Assert.That(_builder.DictionaryToUriEncodedString(null), Is.EqualTo(""));
		}

		[Test]
		public void DictionaryToUriEncodedString_returns_empty_string_when_passed_an_empty_dictionary()
		{
			Assert.That(
				_builder.DictionaryToUriEncodedString(new Dictionary<string, string>()),
				Is.EqualTo(""));
		}

		[Test]
		public void DictionaryToUriEncodedString_creates_a_query_string()
		{
			var parameters = new Dictionary<string, string>
			{
			    {"key1", "value1"},
			    {"key2", "value2"},
				{"key3", "value3"},
			};

			Assert.That(
				_builder.DictionaryToUriEncodedString(parameters),
				Is.EqualTo("key1=value1&key2=value2&key3=value3"));
		}

		[Test]
		public void DictionaryToUriEncodedString_url_encodes_values()
		{
			var parameters = new Dictionary<string, string>
			{
			    {"key1", "one&two yeah"},
			};

			Assert.That(
				_builder.DictionaryToUriEncodedString(parameters),
				Is.EqualTo("key1=one%26two+yeah"));
		}

	}
}
