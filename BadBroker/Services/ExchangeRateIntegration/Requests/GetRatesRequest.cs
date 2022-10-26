using System;
using System.Collections.Generic;
using BadBroker.Models;

namespace BadBroker.Services.ExchangeRateIntegration.Requests
{
    public class GetRatesRequest
    {
        public Currency BaseCurrency { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public List<Currency> CurrenciesForRatesRetrieve { get; set; }
    }
    
    public class GetRatesResponse
    {
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public List<RatesOnDate> Rates { get; set; }
    }
}