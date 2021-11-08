using CEPAggregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Classes.Helpers
{
    public class HelperParsersRegistration
    {
        public ICEPHelperParser[] Parsers = { new BelarusbankHelperParser(), new AgroHelperParser(), new DabrabytHelperParser() };
    }
}
