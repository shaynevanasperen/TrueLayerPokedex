using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fun.AspNetCore.Mvc.Testing;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using TrueLayerPokedex.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace TrueLayerPokedex.Tests
{
	public class Endpoint2Tests : PokemonControllerTests
	{
		public Endpoint2Tests(ExtensibleWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
			: base(factory, testOutputHelper)
		{
		}

		[BddfyFact]
		public void InvalidPokemonName()
		{
			using var scope = App.BeginTestScope();

			this.Given(x => x.APokemonName())
				.And(x => x.PokeApiReturnsNotFoundForPokemonName())
				.When(x => x.RequestingTranslatedInformationForPokemonName())
				.Then(x => x.TheResponseIsNotFound())
				.BDDfy();
		}

		[BddfyTheory]
		[InlineData("cave", false)]
		[InlineData("cave", true)]
		[InlineData("tree", true)]
		[InlineData("tree", false)]
		public void ValidPokemonName(string habitat, bool isLegendary)
		{
			using var scope = App.BeginTestScope();

			var description = Guid.NewGuid().ToString();

			var translated = habitat == "cave" || isLegendary
				? "yoda"
				: "shakespeare";

			this.Given(x => x.APokemonName())
				.And(x => x.PokeApiReturnsSpeciesDataForPokemonName(description, habitat, isLegendary))
				.And(x => x.TranslationApiReturnsShakespeareForPokemonDescription(description))
				.And(x => x.TranslationApiReturnsYodaForPokemonDescription(description))
				.When(x => x.RequestingTranslatedInformationForPokemonName())
				.Then(x => x.TheResponseIsAJsonPokemonSummary(translated, habitat, isLegendary))
				.BDDfy();
		}

		[BddfyTheory]
		[InlineData(HttpStatusCode.NotFound)]
		[InlineData(HttpStatusCode.BadRequest)]
		[InlineData(HttpStatusCode.TooManyRequests)]
		public void ValidPokemonNameTranslationRequestFailed(HttpStatusCode statusCode)
		{
			using var scope = App.BeginTestScope();

			var description = Guid.NewGuid().ToString();
			const string habitat = "cave";
			const bool isLegendary = true;

			this.Given(x => x.APokemonName())
				.And(x => x.PokeApiReturnsSpeciesDataForPokemonName(description, habitat, isLegendary))
				.And(x => x.TranslationApiReturnsStatusForPokemonDescription(description, statusCode))
				.When(x => x.RequestingTranslatedInformationForPokemonName())
				.Then(x => x.TheResponseIsAJsonPokemonSummary(description, habitat, isLegendary))
				.BDDfy();
		}

		[BddfyFact]
		public void ValidPokemonNameTranslationFailed()
		{
			using var scope = App.BeginTestScope();

			var description = Guid.NewGuid().ToString();
			const string habitat = "cave";
			const bool isLegendary = true;

			this.Given(x => x.APokemonName())
				.And(x => x.PokeApiReturnsSpeciesDataForPokemonName(description, habitat, isLegendary))
				.And(x => x.TranslationApiReturnsInvalidResponseForPokemonDescription(description))
				.When(x => x.RequestingTranslatedInformationForPokemonName())
				.Then(x => x.TheResponseIsAJsonPokemonSummary(description, habitat, isLegendary))
				.BDDfy();
		}

		protected void TranslationApiReturnsShakespeareForPokemonDescription(string description) =>
			InterceptHttp.RespondWithJsonFor(async x =>
				x.RequestUri?.ToString() == "https://api.funtranslations.com/translate/shakespeare.json" &&
				await x.Content.HasJsonValue("text", description), new
				{
					success = new
					{
						total = 1
					},
					contents = new
					{
						translated = "shakespeare",
						text = description,
						translation = "shakespeare"
					}
				});

		protected void TranslationApiReturnsYodaForPokemonDescription(string description) =>
			InterceptHttp.RespondWithJsonFor(async x =>
				x.RequestUri?.ToString() == "https://api.funtranslations.com/translate/yoda.json" &&
				await x.Content.HasJsonValue("text", description), new
				{
					success = new
					{
						total = 1
					},
					contents = new
					{
						translated = "yoda",
						text = description,
						translation = "yoda"
					}
				});

		protected void TranslationApiReturnsStatusForPokemonDescription(string description, HttpStatusCode statusCode) =>
			InterceptHttp.RespondWithStatusFor(async x =>
				x.RequestUri?.ToString().StartsWith("https://api.funtranslations.com/translate/") == true &&
				await x.Content.HasJsonValue("text", description), statusCode);

		protected void TranslationApiReturnsInvalidResponseForPokemonDescription(string description) =>
			InterceptHttp.RespondWithJsonFor(async x =>
				x.RequestUri?.ToString().StartsWith("https://api.funtranslations.com/translate/") == true &&
				await x.Content.HasJsonValue("text", description), new
				{
					schema_invalid = true
				});

		async Task RequestingTranslatedInformationForPokemonName()
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, $"/pokemon/translated/{PokemonName}");
			Response = await App.Client.SendAsync(request);
		}
	}
}
