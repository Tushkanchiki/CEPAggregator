using CEPAggregator.Data;
using CEPAggregator.Enums;
using CEPAggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Classes.Helpers
{
    public class PriceCalculator
    {
        private List<ExchangeRate> BelarusRates;
        private List<ExchangeRate> AgroRates;
        private List<ExchangeRate> DabraRates;

        public PriceCalculator(ApplicationDbContext dbContext)
        {
            AgroRates = new AgroRateParser().GetRates(dbContext);
            BelarusRates = new BelarusbankRateParser().GetRates(dbContext);
            DabraRates = new DabrabytRateParser().GetRates(dbContext);
        }


        public double CalcPrice(CurrencyType currency, int count, CEP cep)
        {
            switch (cep.BankName)
            {
                case Constants.AgroBank:
                    var rate1 = AgroRates.First(r => r.Cep == cep && r.Target == currency).Rate;
                    return count * rate1;
                case Constants.Belarusbank:
                    var rate2 = BelarusRates.First(r => r.Cep == cep && r.Target == currency).Rate;
                    return count * rate2;
                case Constants.Dabrabyt:
                    var rate3 = DabraRates.First(r => r.Cep == cep && r.Target == currency).Rate;
                    return count * rate3;
            }
            return 0;
        }
    }
}
