using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TagusAir.Data;
using TagusAir.Models;

namespace TagusAir.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFlightRepository _flightRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IFlightRepository flightRepository)
        {
            _logger = logger;
            _flightRepository = flightRepository;
        }

        public IActionResult Index()
        {
            var airports = _flightRepository.GetComboAirports();
            return View(airports);
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
