using CEPAggregator.Classes;
using CEPAggregator.Classes.Helpers.InfoParsers;
using CEPAggregator.Data;
using CEPAggregator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Controllers
{
    public class CepController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CepController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(int id)
        {
            var cep = _dbContext.CEPs.Include(c => c.Address).ThenInclude(a => a.City).FirstOrDefault(c => c.Id == id);
            if (cep == null)
                return NoContent();

            var dict = GetInfo(cep);
            return View((cep, dict));
        }

        private Dictionary<string, string> GetInfo(CEP cep)
        { 
            switch (cep.BankName)
            {
                case Constants.Belarusbank:
                    BelarusbankInfoParser parser1 = new BelarusbankInfoParser();
                    return parser1.GetInfo(cep);
                case Constants.Dabrabyt:
                    DabrabytInfoParser parser2 = new DabrabytInfoParser();
                    return parser2.GetInfo(cep);
                case Constants.AgroBank:
                    AgroInfoParser parser3 = new AgroInfoParser();
                    return parser3.GetInfo(cep);
            }
            return new Dictionary<string, string>();
        }
    }
}
