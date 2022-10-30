using System;
using System.Collections.Generic;

namespace BadBroker.Models
{
    public class RatesOnDate
    {
        public RatesOnDate(DateTime date, List<Rate> rates)
        {
            Date = date;
            Rates = rates;
        }

        public DateTime Date { get; set; }
        
        public List<Rate> Rates { get; set; }
    }
}