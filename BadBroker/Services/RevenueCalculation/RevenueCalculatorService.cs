using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BadBroker.Models;
using BadBroker.Services.ExchangeRateIntegration;
using BadBroker.Services.ExchangeRateIntegration.Requests;
using BadBroker.Services.RevenueCalculation.Requests;

namespace BadBroker.Services.RevenueCalculation
{
    public class RevenueCalculatorService
    {
        private readonly IExchangeRateIntegrationService _exchangeRateIntegrationService;

        public RevenueCalculatorService(IExchangeRateIntegrationService exchangeRateIntegrationService)
        {
            _exchangeRateIntegrationService = exchangeRateIntegrationService;
        }

        public async Task<CalculateRevenueResponse> CalculateRevenue(CalculateRevenueRequest request)
        {
            const decimal brokerFeeUsd = 1;
            var baseCurrency = Currency.USD;
            
            var currencies = new List<Currency> { Currency.RUB, Currency.EUR, Currency.GBP, Currency.JPY };
            var rateHistory = await _exchangeRateIntegrationService.GetRateHistory(new GetRatesRequest
            {
                BaseCurrency = baseCurrency,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CurrenciesForRatesRetrieve = currencies
            });


            decimal maxCurrencyRevenue = default;
            DateTime buyDate = default;
            DateTime sellDate = default;
            Currency maxRevenueCurrency = default;
            foreach (var currency in currencies)
            {
                var currencyRateHistory = 
                    rateHistory.Rates
                        .Select(i => (i.Date, Rate: i.Rates.FirstOrDefault(r => r.Currency == currency)))
                        .Where(i => i.Rate != null)
                        .ToList();
                for (var i = 0; i < currencyRateHistory.Count - 1; i++)
                {
                    var openCurrencyRate = currencyRateHistory[i];
                    for (var j = i + 1; j < currencyRateHistory.Count; j++)
                    {
                        var closeCurrencyRate = currencyRateHistory[j];
                        var positionOpenedDays = j - i;

                        var currentCurrencyRevenue =
                            openCurrencyRate.Rate.Value * request.MoneyUsd / closeCurrencyRate.Rate.Value - positionOpenedDays * brokerFeeUsd;

                        if (currentCurrencyRevenue > maxCurrencyRevenue)
                        {
                            maxCurrencyRevenue = currentCurrencyRevenue;
                            buyDate = currencyRateHistory[i].Date;
                            sellDate = currencyRateHistory[j].Date;
                            maxRevenueCurrency = currency;
                        }
                    }
                }
            }

            if (maxCurrencyRevenue == default)
                return new CalculateRevenueResponse();

            return new CalculateRevenueResponse
            {
                Revenue = maxCurrencyRevenue,
                Income = maxCurrencyRevenue - request.MoneyUsd,
                BuyDate = buyDate,
                SellDate = sellDate,
                Tool = maxRevenueCurrency,
                RatesOnDate = rateHistory.Rates
            };
        }
    }
}