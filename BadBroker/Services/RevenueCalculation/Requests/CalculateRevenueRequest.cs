using System;
using System.Collections.Generic;
using BadBroker.Models;

namespace BadBroker.Services.RevenueCalculation.Requests
{
    public class CalculateRevenueRequest
    {
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public decimal MoneyUsd { get; set; }
    }
    
    public class CalculateRevenueResponse
    {
        public decimal Revenue { get; set; }
        
        public decimal Income { get; set; }
        
        public DateTime BuyDate { get; set; }
        
        public DateTime SellDate { get; set; }
        
        public Currency Tool { get; set; }
        
        public List<RatesOnDate> RatesOnDate { get; set; }
    }
}