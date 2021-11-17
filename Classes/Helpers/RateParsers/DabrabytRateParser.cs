using CEPAggregator.Data;
using CEPAggregator.Interfaces;
using CEPAggregator.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Xml;

namespace CEPAggregator.Classes
{
    public class DabrabytRateParser : Parser, IRateParser
    {
        private const string ApiURL = "https://bankdabrabyt.by/export_courses.php";

        private ApplicationDbContext dbContext;

        private List<ExchangeRate> ParseXml(string xml)
        {
            List<ExchangeRate> rates = new List<ExchangeRate>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);
            var xRoot = xDoc.DocumentElement;
            var children = xRoot.ChildNodes;
            foreach (XmlNode child in children)
            {
                if (child.Name == "filials")
                {
                    foreach (XmlNode filial in child.ChildNodes)
                    {
                        long customId = 0;
                        bool ok = true;
                        foreach (XmlNode id in filial.SelectNodes("id"))
                        {
                            if (!long.TryParse(id.InnerText, out customId))
                            {
                                ok = false;
                            }
                        }
                        if (!ok)
                        {
                            continue;
                        }
                        var cep = dbContext.CEPs.FirstOrDefault(c => c.CustomId == customId && c.BankName == Constants.Dabrabyt);
                        foreach (XmlNode xmlRates in filial.SelectNodes("rates"))
                        {
                            foreach (XmlNode xmlRate in xmlRates.ChildNodes)
                            {

                                var rate = new ExchangeRate();
                                rate.Cep = cep;
                                rate.Source = Enums.CurrencyType.BYN;
                                ok = true;
                                Enums.CurrencyType target;
                                if (Enum.TryParse(xmlRate.Attributes.GetNamedItem("iso").Value, out target)) {
                                    rate.Target = target;
                                }
                                else
                                {
                                    ok = false;
                                }
                                double doubleRate;
                                if (double.TryParse(xmlRate.Attributes.GetNamedItem("sale").Value, 
                                    NumberStyles.Any, 
                                    CultureInfo.InvariantCulture, 
                                    out doubleRate)) {
                                    rate.Rate = doubleRate;
                                }
                                else
                                {
                                    ok = false;
                                }
                                if (ok)
                                {
                                    rates.Add(rate);
                                }
                            }
                        }
                    }
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
