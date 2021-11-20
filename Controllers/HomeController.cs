using CEPAggregator.Classes;
using CEPAggregator.Classes.Helpers;
using CEPAggregator.Data;
using CEPAggregator.Enums;
using CEPAggregator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace CEPAggregator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _dbContext;

        private readonly PriceCalculator _calc;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, PriceCalculator calc)
        {
            _logger = logger;
            _dbContext = dbContext;
            _calc = calc;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(InputModel input)
        {
            double userX = 0, userY = 0;
            if (input.UseLocation)
            {
                userX = Double.Parse(input.UserX, CultureInfo.InvariantCulture);
                userY = Double.Parse(input.UserY, CultureInfo.InvariantCulture);
            }
            var selector = new CEPSelector(userX, userY, input.Currency, _dbContext);
            var res = selector.SelectCEPs(_dbContext.CEPs.Include(c => c.Address).ThenInclude(a => a.City).ToList(), new CEPSelector.SelectionParams { useLocation = input.UseLocation,
                useRating = input.UseRating,
                useCurrency = input.UseCurrency,
                selectCnt = 20 });
            if (!input.UseRating && !input.UseLocation && !input.UseCurrency)
            {
                return RedirectToAction("All", "Cep");
            }
            var prices = res.Select(r => _calc.CalcPrice(input.Currency, input.Count, r.Cep)).ToList();
            return View("Results", (res, input, prices));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
