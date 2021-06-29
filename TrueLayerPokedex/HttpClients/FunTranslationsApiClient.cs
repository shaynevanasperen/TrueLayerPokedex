using System;
using System.Net.Http;

namespace TrueLayerPokedex.HttpClients
{
	public class FunTranslationsApiClient
	{
		public FunTranslationsApiClient(HttpClient httpClient)
		{
			HttpClient = httpClient;
			HttpClient.BaseAddress = new Uri("https://api.funtranslations.com/translate/");
		}

		public HttpClient HttpClient { get; }
	}
}
