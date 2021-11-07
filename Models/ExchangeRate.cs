using CEPAggregator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public CurrencyType Source { get; set; }
        public CurrencyType Target { get; set; }
        public double Rate { get; set; }
    }
}
