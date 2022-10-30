using System;
using System.Collections;
using System.Collections.Generic;
using BadBroker.Models;
using BadBroker.Services.ExchangeRateIntegration;
using BadBroker.Services.ExchangeRateIntegration.Requests;
using BadBroker.Services.RevenueCalculation;
using BadBroker.Services.RevenueCalculation.Requests;
using Moq;
using Xunit;

namespace UnitTests
{
    public class RevenueCalculationTests
    {
        [Theory]
        [ClassData(typeof(TestData))]
        public async void CalculateRevenueTest(GetRatesResponse ratesResponse, decimal moneyUsd,
            CalculateRevenueResponse expectedResult)
        {
            var exchangeRateServiceMock = new Mock<IExchangeRateIntegrationService>();
            exchangeRateServiceMock
                .Setup(service => service.GetRateHistory(It.IsAny<GetRatesRequest>()))
                .ReturnsAsync(ratesResponse);
            
            var revenueCalculatorService = new RevenueCalculatorService(exchangeRateServiceMock.Object);

            var result = await revenueCalculatorService.CalculateRevenue(new CalculateRevenueRequest
            {
                MoneyUsd = moneyUsd
            });
            
            Assert.Equal(expectedResult.Tool, result.Tool);
            Assert.Equal(expectedResult.Revenue, result.Revenue);
            Assert.Equal(expectedResult.Income, result.Income);
            Assert.Equal(expectedResult.BuyDate, result.BuyDate);
            Assert.Equal(expectedResult.SellDate, result.SellDate);
        }
    }

    public class TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return Test1();
            yield return Test2();
            yield return Test3();

            object[] Test1()
            {
                var startDate = new DateTime(2014, 12, 15);
                return new object[]
                {
                    new GetRatesResponse
                    {
                        Rates = new List<RatesOnDate>
                        {
                            new(startDate.AddDays(0), new List<Rate>
                            {
                                new(Currency.RUB, 60.17m)
                            }),
                            new(startDate.AddDays(1), new List<Rate>
                            {
                                new(Currency.RUB, 72.99m)
                            }),
                            new(startDate.AddDays(2), new List<Rate>
                            {
                                new(Currency.RUB, 66.01m)
                            }),
                            new(startDate.AddDays(3), new List<Rate>
                            {
                                new(Currency.RUB, 61.44m)
                            }),
                            new(startDate.AddDays(4), new List<Rate>
                            {
                                new(Currency.RUB, 59.79m)
                            }),
                            new(startDate.AddDays(5), new List<Rate>
                            {
                                new(Currency.RUB, 59.79m)
                            }),
                            new(startDate.AddDays(6), new List<Rate>
                            {
                                new(Currency.RUB, 59.79m)
                            }),
                            new(startDate.AddDays(7), new List<Rate>
                            {
                                new(Currency.RUB, 54.78m)
                            }),
                            new(startDate.AddDays(8), new List<Rate>
                            {
                                new(Currency.RUB, 54.80m)
                            })
                        }
                    }, 100, new CalculateRevenueResponse
                    {
                        Tool = Currency.RUB,
                        Revenue = 127.24m,
                        Income = 27.24m,
                        BuyDate = new DateTime(2014, 12, 16),
                        SellDate = new DateTime(2014, 12, 22)
                    }
                };
            }

            object[] Test2()
            {
                return new object[]
                {
                    new GetRatesResponse
                    {
                        Rates = new List<RatesOnDate>
                        {
                            new(new DateTime(2012, 01, 05), new List<Rate>
                            {
                                new(Currency.RUB, 40m)
                            }),
                            new(new DateTime(2012, 01, 07), new List<Rate>
                            {
                                new(Currency.RUB, 35m)
                            }),
                            new(new DateTime(2012, 01, 19), new List<Rate>
                            {
                                new(Currency.RUB, 30m)
                            })
                        }
                    }, 50, new CalculateRevenueResponse
                    {
                        Tool = Currency.RUB,
                        Revenue = 55.14m,
                        Income = 5.14m,
                        BuyDate = new DateTime(2012, 01, 05),
                        SellDate = new DateTime(2012, 01, 07)
                    }
                };
            }

            object[] Test3()
            {
                return new object[]
                {
                    new GetRatesResponse
                    {
                        Rates = new List<RatesOnDate>
                        {
                            new(new DateTime(2012, 01, 19), new List<Rate>
                            {
                                new(Currency.RUB, 30m)
                            })
                        }
                    }, 50, new CalculateRevenueResponse
                    {
                        Tool = null,
                        Revenue = 50m,
                        Income = 0m,
                        BuyDate = new DateTime(),
                        SellDate = new DateTime()
                    }
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}