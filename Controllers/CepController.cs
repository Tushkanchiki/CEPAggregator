using CEPAggregator.Classes;
using CEPAggregator.Classes.Helpers.InfoParsers;
using CEPAggregator.Data;
using CEPAggregator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CEPAggregator.Interfaces;

namespace CEPAggregator.Controllers
{
    public class CepController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CepController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            var cep = _dbContext.CEPs.Include(c => c.Address).ThenInclude(a => a.City).FirstOrDefault(c => c.Id == id);
            if (cep == null)
                return NoContent();

            var dict = GetInfo(cep);
            var comments = _dbContext.Comments.Where(c => c.CEP == cep).ToList();
            return View((cep, dict, comments));
        }

        [HttpPost]
        public IActionResult Index(int id, Comment comment)
        {
            _dbContext.Comments.Add(comment);
            _dbContext.SaveChanges();

            return RedirectToAction("Index", id);
        }

        [HttpGet]
        public IActionResult All()
        {
            return View(_dbContext.CEPs.Include(c => c.Address).ThenInclude(a => a.City).ToList());
        }

        private Dictionary<string, string> GetInfo(CEP cep)
        {
            Dictionary<string, string> res;
            IInfoParser infoParser;
            IRateParser rateParser;
            switch (cep.BankName)
            {
                case Constants.Belarusbank:
                    infoParser = new BelarusbankInfoParser();
                    rateParser = new BelarusbankRateParser();
                    break;
                case Constants.Dabrabyt:
                    infoParser = new DabrabytInfoParser();
                    rateParser = new DabrabytRateParser();
                    break;
                case Constants.AgroBank:
                    infoParser = new AgroInfoParser();
                    rateParser = new AgroRateParser();
                    break;
                default:
                    return new Dictionary<string, string>();
            }
            res = infoParser.GetInfo(cep);
            var rates = rateParser.GetRates(_dbContext).Where(rate => rate.Cep == cep).ToList();
            foreach (var rate in rates)
            {
                res[$"{rate.Target}"] = rate.Rate.ToString() + " BYN";
            }
            return res;
        }
    }
}
