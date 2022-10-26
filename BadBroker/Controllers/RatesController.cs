using System.Linq;
using System.Threading.Tasks;
using BadBroker.Controllers.Requests;
using BadBroker.Infrastructure.Exceptions;
using BadBroker.Services.RevenueCalculation;
using BadBroker.Services.RevenueCalculation.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BadBroker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatesController : ControllerBase
    {
        [HttpGet("best")]
        public async Task<BestValueResponse> GetBestValue([FromQuery] BestValueRequest request, 
            [FromServices] RevenueCalculatorService revenueCalculatorService)
        {
            ValidateGetBestValueRequest(request);

            var revenueResponse = await revenueCalculatorService.CalculateRevenue(new CalculateRevenueRequest
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MoneyUsd = request.MoneyUsd
            });
            
            return new BestValueResponse
            {
                Rates = revenueResponse.RatesOnDate.Select(rd => new RatesOnDate
                {
                    Date = rd.Date,
                    Rates = rd.Rates.Select(r => new Rate(r.Currency.ToString(), r.Value)).ToList()
                }).ToList(),
                Revenue = revenueResponse.Revenue,
                Income = revenueResponse.Income,
                Tool = revenueResponse.Tool.ToString(),
                BuyDate = revenueResponse.BuyDate,
                SellDate = revenueResponse.SellDate
            };
        }

        private static void ValidateGetBestValueRequest(BestValueRequest request)
        {
            const int maxPeriodDays = 60;

            if (request.MoneyUsd <= 0)
                throw new ValidationException("Money amount should be positive");
            if (request.StartDate >= request.EndDate)
                throw new ValidationException("Start date should be less than end date");
            if ((request.EndDate - request.StartDate).TotalDays > maxPeriodDays)
                throw new ValidationException("Max trading period is 60 days");
        }
    }
}