using CEPAggregator.Data;
using CEPAggregator.Interfaces;
using CEPAggregator.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Xml;

namespace CEPAggregator.Classes
{
    public class AgroRateParser : Parser, IRateParser
    {
        private const string ApiURL = "https://belapb.by/CashExRatesDaily.php";

        private ApplicationDbContext dbContext;

        private List<ExchangeRate> ParseXml(string xml)
        {
            List<ExchangeRate> rates = new List<ExchangeRate>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);
            var xRoot = xDoc.DocumentElement;
            var xmlRates = xRoot.ChildNodes;

            foreach (XmlNode xmlRate in xmlRates)
            {
                ExchangeRate rate = new ExchangeRate();
                rate.Source = Enums.CurrencyType.BYN;
                rate.Rate = 1;
                long customId = 0;
                bool ok = true;
                foreach (XmlNode node in xmlRate.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "RateSell":
                            double doubleRate = 0;
                            if (double.TryParse(node.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleRate))
                            {
                                rate.Rate *= doubleRate;
                            } 
                            else
                            {
                                ok = false;
                            }
                            break;
                        case "Scale":
                            double scale = 0;
                            if (double.TryParse(node.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out scale))
                            {
                                rate.Rate /= scale;
                            } 
                            else
                            {
                                ok = false;
                            }
                            break;
                        case "BankId":
                            ok &= long.TryParse(node.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out customId);
                            break;
                        case "CharCode":
                            Enums.CurrencyType type;
                            if (System.Enum.TryParse(node.InnerText, out type))
                            {
                                rate.Target = type;
                            } 
                            else
                            {
                                ok = false;
                            }
                            break;
                    }
                }
                if (ok)
                {
                    rate.Cep = dbContext.CEPs.FirstOrDefault(c => c.CustomId == customId && c.BankName == Constants.AgroBank);
                    rates.Add(rate);
                }
            }

            return rates;
        }

        public List<ExchangeRate> GetRates(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            string apiResponse = GetApi(ApiURL, new MediaTypeWithQualityHeaderValue("text/xml"));
            if (apiResponse != null)
            {
                return ParseXml(apiResponse);
            }
            return new List<ExchangeRate>();
        }
    }
}
