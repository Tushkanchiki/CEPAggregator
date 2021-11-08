using CEPAggregator.Interfaces;
using CEPAggregator.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace CEPAggregator.Classes
{
    public class BelarusbankHelperParser : Parser, ICEPHelperParser
    {
        private const string ApiURL = "https://belarusbank.by/api/kursExchange";

        private List<CEPHelper> ParseJson(string json)
        {
            List<CEPHelper> ceps = new List<CEPHelper>();

            dynamic stuff = JArray.Parse(json);
            foreach (var s in stuff)
            {
                CEPHelper cep = new CEPHelper();
                cep.BankName = Constants.Belarusbank;
                cep.CustomId = long.Parse((string)s.filial_id);
                cep.Name = s.filials_text;
                cep.CityName = s.name;
                cep.Address = $"{s.street_type} {s.street}, {s.home_number}";
                ceps.Add(cep);
            }

            return ceps;
        }

        public IList<CEPHelper> GetCEPHelpers()
        {
            string apiResponse = GetApi(ApiURL, new MediaTypeWithQualityHeaderValue("application/json"));
            if (apiResponse != null)
            {
                return ParseJson(apiResponse);
            }
            return new List<CEPHelper>();
        }
    }
}
