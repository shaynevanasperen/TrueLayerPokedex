using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Fun.AspNetCore.Mvc.Testing;
using TrueLayerPokedex.Models;
using TrueLayerPokedex.Tests.TestInfrastructure;
using Xunit.Abstractions;

namespace TrueLayerPokedex.Tests
{
	public abstract class PokemonControllerTests : WebApplicationTests
	{
		protected PokemonControllerTests(ExtensibleWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
			: base(factory, testOutputHelper)
		{
		}

		protected string PokemonName { get; set; }
		protected HttpResponseMessage Response { get; set; }

		protected void APokemonName() => PokemonName = Guid.NewGuid().ToString();

		protected void PokeApiReturnsNotFoundForPokemonName() =>
			InterceptHttp.RespondWithStatusFor(x => x.RequestUri?.ToString() == $"https://pokeapi.co/api/v2/pokemon-species/{PokemonName}", HttpStatusCode.NotFound);

		protected void PokeApiReturnsSpeciesDataForPokemonName(string description, string habitat, bool isLegendary) =>
			InterceptHttp.RespondWithJsonFor(x => x.RequestUri?.ToString() == $"https://pokeapi.co/api/v2/pokemon-species/{PokemonName}", new
			{
				name = PokemonName,
				flavor_text_entries = new[]
				{
					new
					{
						language = new
						{
							name = "en"
						},
						flavor_text = description
					}
				},
				habitat = new
				{
					name = habitat
				},
				is_legendary = isLegendary
			});

		protected void TheResponseIsNotFound() => Response.StatusCode.Should().Be(HttpStatusCode.NotFound);

		protected async Task TheResponseIsAJsonPokemonSummary(string description, string habitat, bool isLegendary)
		{
			Response.IsSuccessStatusCode.Should().BeTrue();

			var summary = await Response.Content.ReadFromJsonAsync<PokemonSummary>();
			summary.Name.Should().Be(PokemonName);
			summary.Description.Should().Be(description);
			summary.Habitat.Should().Be(habitat);
			summary.IsLegendary.Should().Be(isLegendary);
		}
	}
}
