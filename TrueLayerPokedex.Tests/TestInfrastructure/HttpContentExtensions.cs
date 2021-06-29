using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrueLayerPokedex.Tests.TestInfrastructure
{
	static class HttpContentExtensions
	{
		public static async Task<bool> HasJsonValue(this HttpContent content, string name, string value)
		{
			if (content is JsonContent)
			{
				var json = await content.ReadFromJsonAsync<JsonElement>();
				return json.TryGetProperty(name, out var property) && property.GetString() == value;
			}

			return false;
		}
	}
}
