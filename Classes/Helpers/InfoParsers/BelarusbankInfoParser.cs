using CEPAggregator.Interfaces;
using CEPAggregator.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CEPAggregator.Classes.Helpers.InfoParsers
{
    public class BelarusbankInfoParser : Parser, IInfoParser
    {
        private const string Url = "https://belarusbank.by/api/filials_info?city=";
        public Dictionary<string, string> GetInfo(CEP cep)
        {
            string fullUrl = Url + cep.Address.City.Name;
            string apiResponse = GetApi(fullUrl, new MediaTypeWithQualityHeaderValue("application/json"));

            var dict = new Dictionary<string, string>();
            if (apiResponse != null)
            {
                dict["Банк"] = "Беларусбанк";
                dict["Название обменного пункта"] = cep.Name;
                dict["Адрес"] = $"{cep.Address.City.Name}, {cep.Address.Name}";
                dict = dict.Concat(ParseJson(apiResponse, cep)).ToDictionary(x => x.Key, x => x.Value);
                return dict;
            }
            return dict;
        }

        private Dictionary<string, string> ParseJson(string json, CEP cep)
        {
            var dict = new Dictionary<string, string>();
            dynamic stuff = JArray.Parse(json);
            foreach (var s in stuff)
            {
                if (long.Parse((string)s.filial_id) == cep.CustomId)
                {
                    dict["Время работы"] = s.info_worktime;
                    dict["Номер телефона"] = s.phone_info;
                    break;
                }
            }
            return dict;
        }
    }
}
