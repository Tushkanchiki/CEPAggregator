using CEPAggregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Classes.Helpers
{
    public class HelperRateParsersRegistration
    {
        public IRateParser[] Parsers = { new BelarusbankRateParser(), new AgroRateParser(), new DabrabytRateParser() };
    }
}
