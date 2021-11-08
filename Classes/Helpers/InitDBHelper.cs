using CEPAggregator.Data;
using CEPAggregator.Interfaces;
using CEPAggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Classes
{
    public class InitDBHelper
    {
        private List<CEPHelper> _helpers = new List<CEPHelper>();

        private ApplicationDbContext _applicationDbContext;

        private ICEPHelperParser[] _parsers;

        private void LoadHelpers()
        {
            foreach (var parser in _parsers)
                _helpers.AddRange(parser.GetCEPHelpers());
            _helpers.RemoveAll(h => h.CityName == null || h.Address.Length < 5);
        }

        private void AddCities()
        {
            var cities = _helpers.Select(h => h.CityName).Distinct().ToList();
            foreach (var city in cities)
            {
                _applicationDbContext.Cities.Add(new City() { Name = city });
            }
            _applicationDbContext.SaveChanges();
        }

        private void AddAddresses()
        {
            foreach (var helper in _helpers)
            {
                var city = _applicationDbContext.Cities.FirstOrDefault(c => c.Name == helper.CityName);
                if (city != null)
                    _applicationDbContext.Addresses.Add(new Address() { City = city, Name = helper.Address });
            }
            _applicationDbContext.SaveChanges();
        }

        private void AddCEPs()
        {
            foreach (var helper in _helpers)
            {
                var address = _applicationDbContext.Addresses.FirstOrDefault(a => a.Name == helper.Address);
                if (address != null)
                {
                    var cep = new CEP() { Address = address, Name = helper.Name, BankName = helper.BankName, CustomId = helper.CustomId };
                    _applicationDbContext.CEPs.Add(cep);
                }
            }
            _applicationDbContext.SaveChanges();
        }

        public void InitDB(ApplicationDbContext dbContext, ICEPHelperParser[] parsers)
        {
            _applicationDbContext = dbContext;
            _parsers = parsers;

            LoadHelpers();
            AddCities();
            AddAddresses();
            AddCEPs();
        }
    }
}
