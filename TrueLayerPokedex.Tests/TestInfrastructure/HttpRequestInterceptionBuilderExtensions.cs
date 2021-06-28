using System;
using System.ComponentModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JustEat.HttpClientInterception;

namespace TrueLayerPokedex.Tests.TestInfrastructure
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	static class HttpRequestInterceptionBuilderExtensions
	{
		public static HttpRequestInterceptionBuilder WithJsonResponses<T>(
			this HttpRequestInterceptionBuilder builder,
			Func<int, T, CancellationToken, Task> callback,
			T[] content,
			JsonSerializerOptions options = null)
		{
			var counter = new Counter(content.Length);
			T item = default!;

			return builder
				.WithInterceptionCallback(async (_, cancellationToken) =>
				{
					counter.Increment();
					item = content[counter.Value - 1];
					await callback(counter.Value, item, cancellationToken);
				})
				.WithMediaType("application/json")
				.WithContent(() =>
				{
					if (item is Exception exception)
						throw exception;

					return JsonSerializer.SerializeToUtf8Bytes(item, options);
				});
		}

		class Counter
		{
			readonly int _maxValue;

			public Counter(int maxValue) => _maxValue = maxValue;

			public int Value { get; private set; }

			public void Increment()
			{
				if (Value == _maxValue) return;

				Value++;
			}
		}
	}
}
