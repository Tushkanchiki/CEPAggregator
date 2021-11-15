using CEPAggregator.Classes;
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
            var cep = _dbContext.CEPs.FirstOrDefault(c => c.Id == id);
            if (cep == null)
                return NoContent();

            return View((cep, new Dictionary<string, string>() { {"aboba", "hi" }, {"aboba2", "hi2" } }));
        }
    }
}
