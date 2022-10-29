using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BadBroker.Configs;
using BadBroker.Models;
using BadBroker.Services.ExchangeRateIntegration.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BadBroker.Services.ExchangeRateIntegration.ExchangeRatesApiIoIntegration
{
    public class ExchangeRatesApiIoIntegrationService : IExchangeRateIntegrationService
    {
        private readonly ILogger<ExchangeRatesApiIoIntegrationService> _logger;
        private readonly ExchangeRatesApiIoOptions _apiConfig;
        private const string DateFormat = "yyyy-MM-dd";
        
        private readonly HttpClient _httpClient;

        public ExchangeRatesApiIoIntegrationService(ILogger<ExchangeRatesApiIoIntegrationService> logger,
            IOptions<ExchangeRatesApiIoOptions> apiConfig)
        {
            _logger = logger;
            _apiConfig = apiConfig.Value ?? throw new ArgumentNullException(nameof(apiConfig));
            _httpClient = new HttpClient();
        }

        public async Task<GetRatesResponse> GetRateHistory(GetRatesRequest request)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                RequestUri = GetUri(request),
                Headers = { { "apikey", _apiConfig.ApiKey } },
                Method = HttpMethod.Get
            };

            TimeseriesResponse httpResponse;
            try
            {
                var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);
                var rawHttpResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                httpResponse = JsonSerializer.Deserialize<TimeseriesResponse>(rawHttpResponse);

                if (!httpResponseMessage.IsSuccessStatusCode)
                    throw new Exception($"Bad http response: {httpResponse.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during rates retrieve");
                throw;
            }

            return GetRatesResult(httpResponse);
        }

        private static Uri GetUri(GetRatesRequest request)
        {
            var currenciesToRetrieveRaw = string.Join(",", request.CurrenciesForRatesRetrieve);
            return new Uri(
                $"https://api.apilayer.com/exchangerates_data/timeseries" +
                $"?base={request.BaseCurrency}" +
                $"&start_date={request.StartDate.ToString(DateFormat)}" +
                $"&end_date={request.EndDate.ToString(DateFormat)}" +
                $"&symbols={currenciesToRetrieveRaw}");
        }

        private static GetRatesResponse GetRatesResult(TimeseriesResponse httpResponse)
        {
            return new GetRatesResponse
            {
                StartDate = httpResponse.StartDate,
                EndDate = httpResponse.EndDate,
                Rates = httpResponse.RatesOnDate.Select(kvp => new RatesOnDate
                    {
                        Date = kvp.Key,
                        Rates = kvp.Value.CurrencyToRateDictionary.Select(v => new Rate
                        {
                            Currency = Currency.GetByCode(v.Key),
                            Value = v.Value.GetDecimal()
                        }).ToList()
                    })
                    .OrderBy(i => i.Date)
                    .ToList()
            };
        }

        private class ResponseBase
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }
        }

        private class TimeseriesResponse : ResponseBase
        {
            [JsonPropertyName("start_date")]
            public DateTime StartDate { get; set; }
            
            [JsonPropertyName("end_date")]
            public DateTime EndDate { get; set; }
            
            [JsonPropertyName("success")]
            public bool Success { get; set; }
            
            [JsonPropertyName("rates")]
            public Dictionary<DateTime, CurrencyRates> RatesOnDate { get; set; }
        }
        
        private class CurrencyRates
        {
            [JsonExtensionData]
            public Dictionary<string, JsonElement> CurrencyToRateDictionary { get; set; }
        }
    }
}