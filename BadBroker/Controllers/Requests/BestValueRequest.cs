using System;
using System.Collections.Generic;

namespace BadBroker.Controllers.Requests
{
    public class BestValueRequest
    {
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public decimal MoneyUsd { get; set; }
    }
    
    public class BestValueResponse
    {
        public List<RatesOnDate> Rates { get; set; }
        
        public decimal Revenue { get; set; }
        
        public decimal Income { get; set; }

        public string Tool { get; set; }
        
        public DateTime BuyDate { get; set; }
        
        public DateTime SellDate { get; set; }
    }
    
    public class Rate
    {
        public Rate()
        {
        }

        public Rate(string currency, decimal value)
        {
            Currency = currency;
            Value = value;
        }

        public string Currency { get; set; }
        
        public decimal Value { get; set; }
    }
    
    public class RatesOnDate
    {
        public DateTime Date { get; set; }
        
        public List<Rate> Rates { get; set; }
    }
}