using CEPAggregator.Data;
using CEPAggregator.Models;
using System.Collections.Generic;

namespace CEPAggregator.Interfaces
{
    public interface IRateParser
    {
        public List<ExchangeRate> GetRates(ApplicationDbContext dbContext);
    }
}
