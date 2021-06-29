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
		public async Task<ActionResult<PokemonSummary>> Get(string name, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Getting pokemon summary for: {name}");

			var summary = await _magneto.QueryAsync(new PokemonSummaryByName(name), CacheOption.Default, cancellationToken);

			if (summary == null)
				return NotFound();

			return summary;
		}
	}
}
