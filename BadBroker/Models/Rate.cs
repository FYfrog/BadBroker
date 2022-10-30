namespace BadBroker.Models
{
    public class Rate
    {
        public Rate(Currency currency, decimal value)
        {
            Currency = currency;
            Value = value;
        }

        public Currency Currency { get; set; }
        
        public decimal Value { get; set; }
    }
}