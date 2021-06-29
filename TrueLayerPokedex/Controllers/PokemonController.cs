using System;
using System.Threading;
using System.Threading.Tasks;
using Magneto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrueLayerPokedex.Models;
using TrueLayerPokedex.Queries;

namespace TrueLayerPokedex.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PokemonController : ControllerBase
	{
		readonly ILogger<PokemonController> _logger;
		readonly IMagneto _magneto;

		public PokemonController(ILogger<PokemonController> logger, IMagneto magneto)
		{
			_logger = logger;
			_magneto = magneto;
		}

		[HttpGet("{name}")]
		public async Task<ActionResult<PokemonSummary>> GetBasic(string name, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Getting basic pokemon summary for: {name}");

			var summary = await _magneto.QueryAsync(new PokemonSummaryByName(name), CacheOption.Default, cancellationToken);

			if (summary == null)
				return NotFound();

			return summary;
		}

		[HttpGet("translated/{name}")]
		public async Task<ActionResult<PokemonSummary>> GetTranslated(string name, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Getting translated pokemon summary for: {name}");

			var summary = await _magneto.QueryAsync(new PokemonSummaryByName(name), CacheOption.Default, cancellationToken);

			if (summary == null)
				return NotFound();

			var translationType = summary.Habitat == "cave" || summary.IsLegendary
				? "yoda"
				: "shakespeare";

			try
			{
				var translation = await _magneto.QueryAsync(new TranslationByTypeAndInput(translationType, summary.Description), CacheOption.Default, cancellationToken);
				if (translation != null)
					summary.Description = translation;
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, $"Failed to get {translationType} translation for \"{summary.Description}\".");
			}

			return summary;
		}
	}
}
