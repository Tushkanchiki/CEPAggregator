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
    public class DabrabytHelperParser : Parser, ICEPHelperParser
    {
        private const string ApiURL = "https://bankdabrabyt.by/export_courses.php";

        private List<CEPHelper> ParseXml(string xml)
        {
            List<CEPHelper> ceps = new List<CEPHelper>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);
            var xRoot = xDoc.DocumentElement;
            var filialsNode = xRoot.ChildNodes.Item(1);
            foreach(XmlNode filial in filialsNode.ChildNodes)
            {
                if (filial.Name == "Курсы по карточкам")
                    continue;

                var cep = new CEPHelper();
                cep.Name = HttpUtility.HtmlDecode(filial.Attributes.GetNamedItem("name").Value);
                cep.BankName = Constants.Dabrabyt;

                foreach (XmlNode xNode in filial.ChildNodes)
                {
                    switch (xNode.Name)
                    {
                        case "city":
                            cep.CityName = HttpUtility.HtmlDecode(xNode.InnerText);
                            break;
                        case "address":
                            cep.Address = HttpUtility.HtmlDecode(xNode.InnerText);
                            break;
                        case "id":
                            cep.CustomId = long.Parse(xNode.InnerText);
                            break;
                    }
                }

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
