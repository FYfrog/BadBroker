using System.Threading.Tasks;
using BadBroker.Services.ExchangeRateIntegration.Requests;

namespace BadBroker.Services.ExchangeRateIntegration
{
    public interface IExchangeRateIntegrationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Rates per day ordered by date</returns>
        Task<GetRatesResponse> GetRateHistory(GetRatesRequest request);
    }
}