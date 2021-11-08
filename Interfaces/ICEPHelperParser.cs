using CEPAggregator.Models;
using System.Collections.Generic;

namespace CEPAggregator.Interfaces
{
    public interface ICEPHelperParser
    {
        public IList<CEPHelper> GetCEPHelpers();
    }
}
