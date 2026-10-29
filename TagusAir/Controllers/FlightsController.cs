using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TagusAir.Data;
using TagusAir.Helpers;
using TagusAir.Models;

namespace TagusAir.Controllers
{
    public class FlightsController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ICountryRepository _countryRepository;
        private readonly ITicketRepository _ticketRepository;

        public FlightsController(
            IFlightRepository flightRepository,
            IConverterHelper converterHelper,
            ICountryRepository countryRepository,
            ITicketRepository ticketRepository
            )
        {
            _flightRepository = flightRepository;
            _converterHelper = converterHelper;
            _countryRepository = countryRepository;
            _ticketRepository = ticketRepository;
        }
        public IActionResult Index()
        {
            return View(_flightRepository.GetAvailableFlights());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetFlightWithDetailsAsync(id.Value);

            if (flight == null)
            {

                return NotFound();
            }

            return View(flight);


        }


        [Authorize(Roles = "Employee")]
        public IActionResult Create()
        {
            return View(new FlightViewModel
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                Airplanes = _flightRepository.GetComboAirplanes(),
                Countries = _countryRepository.GetComboCountries()

            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create(FlightViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var flight = _converterHelper.ToFlight(viewModel, true);
                await _flightRepository.CreateAsync(flight);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Airplanes = _flightRepository.GetComboAirplanes();
            viewModel.Countries = _countryRepository.GetComboCountries();
            viewModel.DepartureAirports = _countryRepository.GetComboAirports(viewModel.DepartureCountryId);
            viewModel.ArrivalAirports = _countryRepository.GetComboAirports(viewModel.ArrivalCountryId);

            return View(viewModel);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetFlightWithDetailsAsync(id.Value);

            if (flight == null)
            {
                return NotFound();
            }

            var viewModel = _converterHelper.ToFlightViewModel(flight);

            viewModel.DepartureCountryId = flight.DepartureAirport.CountryId;
            viewModel.ArrivalCountryId = flight.ArrivalAirport.CountryId;

            viewModel.Airplanes = _flightRepository.GetComboAirplanes();
            viewModel.Countries = _countryRepository.GetComboCountries();
            viewModel.DepartureAirports = _countryRepository.GetComboAirports(viewModel.DepartureCountryId);
            viewModel.ArrivalAirports = _countryRepository.GetComboAirports(viewModel.ArrivalCountryId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(FlightViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var flight = _converterHelper.ToFlight(viewModel, false);
                await _flightRepository.UpdateAsync(flight);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Airplanes = _flightRepository.GetComboAirplanes();
            viewModel.DepartureAirports = _flightRepository.GetComboAirports();
            viewModel.ArrivalAirports = _flightRepository.GetComboAirports();

            return View(viewModel);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var flight = await _flightRepository.GetFlightWithDetailsAsync(id.Value);

            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _flightRepository.GetFlightWithDetailsAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            var flightHasTickets = await _ticketRepository.FlightHasTicketsAsync(id);

            if (flightHasTickets)
            {
                ModelState.AddModelError(string.Empty, "This flight has sold tickets and cannot be deleted.");
                return View(flight);
            }

            await _flightRepository.DeleteAsync(flight);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("Flights/GetAirportsAsync")]
        public async Task<JsonResult> GetAirportsAsync(int countryId)
        {
            var country = await _countryRepository.GetCountryWithAirportsAsync(countryId);

            return Json(country.Airports
                .OrderBy(a => a.City)
                .Select(a => new { a.Id, a.City, a.Name, a.IataCode }));
        }
    }
}
