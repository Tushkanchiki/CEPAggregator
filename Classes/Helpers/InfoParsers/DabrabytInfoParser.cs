using CEPAggregator.Interfaces;
using CEPAggregator.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace CEPAggregator.Classes
{
    public class DabrabytInfoParser : Parser, IInfoParser
    {
        private const string ApiURL = "https://bankdabrabyt.by/api/departments.php";

        private Dictionary<string, string> ParseXml(string xml, CEP cep)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);
            var xRoot = xDoc.DocumentElement;

            foreach(XmlNode filial in xRoot.ChildNodes)
            {
                var dict = new Dictionary<string, string>();
                bool isSearched = true;
                foreach (XmlNode xNode in filial.ChildNodes)
                {
                    switch (xNode.Name)
                    {
                        case "time":
                            dict["Время работы"] = HttpUtility.HtmlDecode(xNode.InnerText);
                            break;
                        case "phones":
                            dict["Информация"] = HttpUtility.HtmlDecode(xNode.InnerText).Replace("<br>", "");
                            break;
                        case "name":
                            if (HttpUtility.HtmlDecode(xNode.InnerText) != cep.Name)
                            { 
                                isSearched = false;
                            }
                            break;
                    }
                }
                if (isSearched)
                    return dict;
            }

            return new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetInfo(CEP cep)
        {
            var dict = new Dictionary<string, string>();
            dict["Банк"] = "Банк Дабрабыт";
            dict["Название обменного пункта"] = cep.Name;
            dict["Адрес"] = $"{cep.Address.City.Name}, {cep.Address.Name}";

            string apiResponse = GetApi(ApiURL, new MediaTypeWithQualityHeaderValue("text/xml"));
            if (apiResponse != null)
            {
                return dict.Concat(ParseXml(apiResponse, cep)).ToDictionary(x => x.Key, x => x.Value);
            }
            return dict;
        }
    }
}
