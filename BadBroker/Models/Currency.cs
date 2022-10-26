using System;

namespace BadBroker.Models
{
    public sealed class Currency
    {
        public static Currency USD = new("USD");
        public static Currency RUB = new("RUB");
        public static Currency EUR = new("EUR");
        public static Currency GBP = new("GBP");
        public static Currency JPY = new("JPY");
        
        private Currency(string code)
        {
            Code = code;
        }

        public string Code { get; }

        public static Currency GetByCode(string currencyCode)
        {
            if (currencyCode == null)
                throw new ArgumentNullException(nameof(currencyCode));

            var currency = currencyCode.ToUpper() switch
            {
                "USD" => USD,
                "RUB" => RUB,
                "EUR" => EUR,
                "GBP" => GBP,
                "JPY" => JPY,
                _ => throw new ArgumentOutOfRangeException($"unknown currency: {nameof(currencyCode)}")
            };

            return currency;
        }

        public override string ToString()
        {
            return Code;
        }
    }
}