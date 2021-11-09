using CEPAggregator.Data;
using CEPAggregator.Interfaces;
using CEPAggregator.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;

namespace CEPAggregator.Classes
{
    public class BelarusbankRateParser : Parser, IRateParser
    {
        private const string ApiURL = "https://belarusbank.by/api/kursExchange";

        private ApplicationDbContext dbContext;

        private List<ExchangeRate> ParseJson(string json)
        {
            List<ExchangeRate> rates = new List<ExchangeRate>();

            dynamic stuff = JArray.Parse(json);
            foreach (var s in stuff)
            {
                long customId = long.Parse((string)s.filial_id);

                var cep = dbContext.CEPs.FirstOrDefault(c => c.CustomId == customId && c.BankName == Constants.Belarusbank);
                if (cep != null)
                {
                    double doubleRate;
                    if (double.TryParse((string)s.USD_out, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleRate))
                    {
                        ExchangeRate rate = new ExchangeRate();
                        rate.Cep = cep;
                        rate.Source = Enums.CurrencyType.BYN;
                        rate.Target = Enums.CurrencyType.USD;
                        rate.Rate = doubleRate / 100;
                        rates.Add(rate);
                    }
                    if (double.TryParse((string)s.EUR_out, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleRate))
                    {
                        ExchangeRate rate = new ExchangeRate();
                        rate.Cep = cep;
                        rate.Source = Enums.CurrencyType.BYN;
                        rate.Target = Enums.CurrencyType.EUR;
                        rate.Rate = doubleRate / 100;
                        rates.Add(rate);
                    }
                    if (double.TryParse((string)s.RUB_out, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleRate))
                    {
                        ExchangeRate rate = new ExchangeRate();
                        rate.Cep = cep;
                        rate.Source = Enums.CurrencyType.BYN;
                        rate.Target = Enums.CurrencyType.RUB;
                        rate.Rate = doubleRate / 100;
                        rates.Add(rate);
                    }
                }
            }

            return rates;
        }

        public List<ExchangeRate> GetRates(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            string apiResponse = GetApi(ApiURL, new MediaTypeWithQualityHeaderValue("application/json"));
            if (apiResponse != null)
            {
                return ParseJson(apiResponse);
            }
            return new List<ExchangeRate>();
        }
    }
}
