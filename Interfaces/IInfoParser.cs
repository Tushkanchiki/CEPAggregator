using CEPAggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Interfaces
{
    public interface IInfoParser
    {
        public Dictionary<string, string> GetInfo(CEP cep);
    }
}
