using CEPAggregator.Classes.Helpers;
using CEPAggregator.Data;
using CEPAggregator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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
            var res = selector.SelectCEPs(_dbContext.CEPs.ToList(), new CEPSelector.SelectionParams { useLocation = input.UseLocation,
                useRating = input.UseRating,
                useCurrency = input.UseCurrency,
                selectCnt = 20 });
            return View("Results", res);
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
