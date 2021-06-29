# TrueLayer Software Engineering Challenge

This repository contains the solution for the "Pokedex" coding challenge put forth by [TrueLayer](https://truelayer.com/).


## How to run

1. Install [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0).
2. Clone this repository.
3. Navigate to: `<cloned folder>\TrueLayerPokedex`
4. Run the CLI command: `dotnet run`
5. Navigate to `http://localhost:5090/pokemon/<name of pokemon>` in your web browser.
6. Navigate to `http://localhost:5090/pokemon/translated/<name of pokemon>` in your web browser.


## Comments for reviewer

I chose to use my own NuGet package named [Magneto](https://github.com/shaynevanasperen/Magneto) for this solution.
This is because it neatly abstracts the queries as objects that adhere to a common interface so that we can add cross-cutting concerns
such as logging/caching easily. It also promotes an architecture that is devoid of the `XYZService` type classes which tend to
become god objects containing several unrelated methods. It's very similar to the well-known [MediatR](https://github.com/jbogard/MediatR)
library, except with lower *ceremony* (since the commmand/query is also the handler).

I also prefer to write tests in the way you will find in this repository. They're completely end-to-end, with "mocking" done at the HTTP layer.
This affords me the greatest flexibility to change the implementation without breaking my tests.

**Things I would do differently for a production applicaton:**
- Add retry, timeout and circuit-breaker HTTP handlers (using [Polly](https://github.com/App-vNext/Polly)).
- Add API documentation (using [Swagger](https://swagger.io/)).
- Improve detail in logging and possibly add more logging coverage.
- Stream log files to a central repostitory (such as [Logstash](https://www.elastic.co/logstash/)).
- Track metrics to an analytics storage service (such as [ApplicationInsights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core)).
- Add HTTP cache headers to responses to indicate that they can be cached.
- Consider adding output caching in addition to the caching happening inside our query objects.
- Structure the project differently, so that related functionality is kept close together.
