using CEPAggregator.Interfaces;
using CEPAggregator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Xml;

namespace CEPAggregator.Classes
{
    public class AgroInfoParser : Parser, IInfoParser
    {
        private const string ApiURL = "https://belapb.by/ExBanks.php";

        private Dictionary<string, string> ParseXml(string xml, CEP cep)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);
            var xRoot = xDoc.DocumentElement;
            var filials = xRoot.ChildNodes;

            foreach (XmlNode filial in filials)
            {
                var filialChilds = filial.ChildNodes;
                bool isSearchedNode = true;
                var dict = new Dictionary<string, string>();

                foreach (XmlNode node in filialChilds)
                {
                    switch (node.Name)
                    {
                        case "BankWorkTimeRu":
                            dict["Время работы"] = node.InnerText;
                            break;
                        case "BankPhone":
                            dict["Номер телефона"] = node.InnerText;
                            break;
                        case "BankServisesRu":
                            if (!string.IsNullOrEmpty(node.InnerText))
                            {
                                dict["Услуги"] = node.InnerText;
                            }
                            break;
                        case "BankTitleRu":
                            if (cep.Name != node.InnerText)
                                isSearchedNode = false;
                            break;
                    }
                }

                if (isSearchedNode)
                {
                    return dict;
                }
            }

            return new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetInfo(CEP cep)
        {
            var dict = new Dictionary<string, string>();
            dict["Банк"] = "Белагропромбанк";
            dict["Название обменного пункта"] = cep.Name;
            dict["Адрес"] = $"{cep.Address.City.Name}, {cep.Address.Name}";

            string apiResponse = GetApi(ApiURL, new MediaTypeWithQualityHeaderValue("text/xml"));
            if (apiResponse != null)
            {
                return dict.Concat(ParseXml(apiResponse, cep)).ToDictionary(x => x.Key, x => x.Value);
            }
            return new Dictionary<string, string>();
        }
    }
}
