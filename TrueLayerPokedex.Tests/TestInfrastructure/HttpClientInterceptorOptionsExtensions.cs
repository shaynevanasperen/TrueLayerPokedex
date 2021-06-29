using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JustEat.HttpClientInterception;

namespace TrueLayerPokedex.Tests.TestInfrastructure
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	static class HttpClientInterceptorOptionsExtensions
	{
		static readonly Random Random = new();

		public static void RespondWithStatusFor(this HttpClientInterceptorOptions options, Predicate<HttpRequestMessage> predicate, HttpStatusCode statusCode) =>
			options.RespondWithStatusFor(x => Task.FromResult(predicate(x)), statusCode);

		public static void RespondWithStatusFor(this HttpClientInterceptorOptions options, Func<HttpRequestMessage, Task<bool>> predicate, HttpStatusCode statusCode) =>
			new HttpRequestInterceptionBuilder()
				.For(predicate)
				.Responds()
				.WithStatus(statusCode)
				.RegisterWith(options);

		public static void RespondWithJsonFor<T>(this HttpClientInterceptorOptions options, Predicate<HttpRequestMessage> predicate, params T[] responses) =>
			options.RespondWithJsonFor(predicate, TimeSpan.FromMilliseconds(Random.Next(10, 20)), responses);

		public static void RespondWithJsonFor<T>(this HttpClientInterceptorOptions options, Predicate<HttpRequestMessage> predicate, TimeSpan delay, params T[] responses) =>
			options.RespondWithJsonFor(predicate, (_, _, cancellationToken) => Task.Delay(delay, cancellationToken), responses);

		public static void RespondWithJsonFor<T>(this HttpClientInterceptorOptions options, Predicate<HttpRequestMessage> predicate, Action<int, T> callback, params T[] responses) =>
			options.RespondWithJsonFor(predicate, (number, item, _) =>
			{
				callback(number, item);
				return Task.CompletedTask;
			}, responses);

		public static void RespondWithJsonFor<T>(this HttpClientInterceptorOptions options, Predicate<HttpRequestMessage> predicate, Func<int, T, CancellationToken, Task> callback, params T[] responses) =>
			new HttpRequestInterceptionBuilder()
				.For(predicate)
				.WithJsonResponses(callback, responses)
				.RegisterWith(options);

		public static void RespondWithJsonFor<T>(this HttpClientInterceptorOptions options, Func<HttpRequestMessage, Task<bool>> predicate, params T[] responses) =>
			options.RespondWithJsonFor(predicate, TimeSpan.FromMilliseconds(Random.Next(10, 20)), responses);

		public static void RespondWithJsonFor<T>(this HttpClientInterceptorOptions options, Func<HttpRequestMessage, Task<bool>> predicate, TimeSpan delay, params T[] responses) =>
			options.RespondWithJsonFor(predicate, (_, _, cancellationToken) => Task.Delay(delay, cancellationToken), responses);

		public static void RespondWithJsonFor<T>(this HttpClientInterceptorOptions options, Func<HttpRequestMessage, Task<bool>> predicate, Action<int, T> callback, params T[] responses) =>
			options.RespondWithJsonFor(predicate, (number, item, _) =>
			{
				callback(number, item);
				return Task.CompletedTask;
			}, responses);

		public static void RespondWithJsonFor<T>(this HttpClientInterceptorOptions options, Func<HttpRequestMessage, Task<bool>> predicate, Func<int, T, CancellationToken, Task> callback, params T[] responses) =>
			new HttpRequestInterceptionBuilder()
				.For(predicate)
				.WithJsonResponses(callback, responses)
				.RegisterWith(options);
	}
}
