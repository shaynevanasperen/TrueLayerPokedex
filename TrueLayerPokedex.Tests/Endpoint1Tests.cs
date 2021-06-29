using System.Net.Http;
using System.Threading.Tasks;
using Fun.AspNetCore.Mvc.Testing;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using Xunit.Abstractions;

namespace TrueLayerPokedex.Tests
{
	public class Endpoint1Tests : PokemonControllerTests
	{
		public Endpoint1Tests(ExtensibleWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
			: base(factory, testOutputHelper)
		{
		}

		[BddfyFact]
		public void InvalidPokemonName()
		{
			using var scope = App.BeginTestScope();

			this.Given(x => x.APokemonName())
				.And(x => x.PokeApiReturnsNotFoundForPokemonName())
				.When(x => x.RequestingBasicInformationForPokemonName())
				.Then(x => x.TheResponseIsNotFound())
				.BDDfy();
		}

		[BddfyFact]
		public void ValidPokemonName()
		{
			using var scope = App.BeginTestScope();

			const string description = "description";
			const string habitat = "habitat";
			const bool isLegendary = true;

			this.Given(x => x.APokemonName())
				.And(x => x.PokeApiReturnsSpeciesDataForPokemonName(description, habitat, isLegendary))
				.When(x => x.RequestingBasicInformationForPokemonName())
				.Then(x => x.TheResponseIsAJsonPokemonSummary(description, habitat, isLegendary))
				.BDDfy();
		}

		async Task RequestingBasicInformationForPokemonName()
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, $"/pokemon/{PokemonName}");
			Response = await App.Client.SendAsync(request);
		}
	}
}
