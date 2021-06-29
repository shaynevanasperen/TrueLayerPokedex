using System;
using System.Net.Http;

namespace TrueLayerPokedex.HttpClients
{
	public class PokeApiClient
	{
		public PokeApiClient(HttpClient httpClient)
		{
			HttpClient = httpClient;
			HttpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
		}

		public HttpClient HttpClient { get; }
	}
}
