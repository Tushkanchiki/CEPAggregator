using CEPAggregator.Interfaces;
using CEPAggregator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Xml;

namespace CEPAggregator.Classes
{
    public class AgroHelperParser : Parser, ICEPHelperParser
    {
        private const string ApiURL = "https://belapb.by/ExBanks.php";

        private List<CEPHelper> ParseXml(string xml)
        {
            List<CEPHelper> ceps = new List<CEPHelper>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);
            var xRoot = xDoc.DocumentElement;
            var filials = xRoot.ChildNodes;

            foreach (XmlNode filial in filials)
            {
                bool isCep = true;
                var cep = new CEPHelper();
                cep.BankName = Constants.AgroBank;
                cep.CustomId = long.Parse(filial.Attributes.GetNamedItem("Id").Value);

                var filialChilds = filial.ChildNodes;
                foreach (XmlNode node in filialChilds)
                {
                    switch (node.Name)
                    {
                        case "BankType":
                            if (node.InnerText != "Точки продаж")
                                isCep = false;
                            break;
                        case "CityTitleRu":
                            cep.CityName = node.InnerText;
                            break;
                        case "BankTitleRu":
                            cep.Name = node.InnerText;
                            break;
                        case "BankAddressRu":
                            cep.Address = node.InnerText;
                            break;
                    }
                }

                if (isCep)
                    ceps.Add(cep);
            }

            return ceps;
        }

        public IList<CEPHelper> GetCEPHelpers()
        {
            string apiResponse = GetApi(ApiURL, new MediaTypeWithQualityHeaderValue("text/xml"));
            if (apiResponse != null)
            {
                return ParseXml(apiResponse);
            }
            return new List<CEPHelper>();
        }
    }
}
