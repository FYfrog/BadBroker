using System;
using System.Collections.Generic;

namespace BadBroker.Models
{
    public class RatesOnDate
    {
        public DateTime Date { get; set; }
        
        public List<Rate> Rates { get; set; }
    }
}