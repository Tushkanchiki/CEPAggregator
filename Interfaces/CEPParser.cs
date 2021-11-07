using CEPAggregator.Models;
using System.Collections.Generic;

namespace CEPAggregator.Interfaces
{
    interface CEPParser
    {
        public IList<CEP> GetCEPs();
    }
}
