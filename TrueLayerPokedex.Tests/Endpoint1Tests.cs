using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Fun.AspNetCore.Mvc.Testing;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using TrueLayerPokedex.Models;
using TrueLayerPokedex.Tests.TestInfrastructure;
using Xunit.Abstractions;

namespace TrueLayerPokedex.Tests
{
	public class Endpoint1Tests : WebApplicationTests
	{
		string _pokemonName;
		HttpResponseMessage _response;

		public Endpoint1Tests(ExtensibleWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
			: base(factory, testOutputHelper)
		{
		}

		[BddfyFact]
		void InvalidPokemonName()
		{
			using var scope = App.BeginTestScope();

			this.Given(x => x.APokemonName())
				.And(x => x.PokeApiReturnsNotFoundForPokemonName())
				.When(x => x.RequestingBasicInformationForPokemonName())
				.Then(x => x.TheResponseIsNotFound())
				.BDDfy();
		}

		[BddfyFact]
		void ValidPokemonName()
		{
			using var scope = App.BeginTestScope();

			this.Given(x => x.APokemonName())
				.And(x => x.PokeApiReturnsSpeciesDataForPokemonName())
				.When(x => x.RequestingBasicInformationForPokemonName())
				.Then(x => x.TheResponseIsAJsonPokemonSummary())
				.BDDfy();
		}

		void APokemonName() => _pokemonName = Guid.NewGuid().ToString();

		void PokeApiReturnsNotFoundForPokemonName() => InterceptHttp.RespondWithStatusFor(x => x.RequestUri?.ToString() == $"https://pokeapi.co/api/v2/pokemon-species/{_pokemonName}", HttpStatusCode.NotFound);

		void PokeApiReturnsSpeciesDataForPokemonName() => InterceptHttp.RespondWithJsonFor(x => x.RequestUri?.ToString() == $"https://pokeapi.co/api/v2/pokemon-species/{_pokemonName}", new
		{
			name = _pokemonName,
			flavor_text_entries = new[]
			{
				new
				{
					language = new
					{
						name = "en"
					},
					flavor_text = "description"
				}
			},
			habitat = new
			{
				name = "habitat"
			},
			is_legendary = true
		});

		async Task RequestingBasicInformationForPokemonName()
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, $"/pokemon/{_pokemonName}");
			_response = await App.Client.SendAsync(request);
		}

		void TheResponseIsNotFound() => _response.StatusCode.Should().Be(HttpStatusCode.NotFound);

		async Task TheResponseIsAJsonPokemonSummary()
		{
			_response.IsSuccessStatusCode.Should().BeTrue();

			var summary = await _response.Content.ReadFromJsonAsync<PokemonSummary>();
			summary.Name.Should().Be(_pokemonName);
			summary.Description.Should().Be("description");
			summary.Habitat.Should().Be("habitat");
			summary.IsLegendary.Should().BeTrue();
		}
	}
}
