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
            if (this.User.IsInRole("Employee"))
            {
                return View(_flightRepository.GetUpcomingFlights());
            }

            return View(_flightRepository.GetAvailableFlights());
        }

        public IActionResult Search(DateTime? date, int? originAirportId, int? destinationAirportId)
        {
            var flights = _flightRepository.SearchFlights(date, originAirportId, destinationAirportId);

            return View("Index", flights);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetFlightWithDetailsAsync(id.Value);

            if (flight == null)
            {

                return new NotFoundViewResult("FlightNotFound");
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
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetFlightWithDetailsAsync(id.Value);

            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");

            }

            var viewModel = _converterHelper.ToFlightViewModel(flight);

            viewModel.DepartureCountryId = flight.DepartureAirport.CountryId;
            viewModel.ArrivalCountryId = flight.ArrivalAirport.CountryId;

            viewModel.Airplanes = _flightRepository.GetComboAirplanes();
            viewModel.Countries = _countryRepository.GetComboCountries();
            viewModel.DepartureAirports = _countryRepository.GetComboAirports(viewModel.DepartureCountryId);
            viewModel.ArrivalAirports = _countryRepository.GetComboAirports(viewModel.ArrivalCountryId);

            viewModel.HasTickets = await _ticketRepository.FlightHasTicketsAsync(flight.Id);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(FlightViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var hasTickets = await _ticketRepository.FlightHasTicketsAsync(viewModel.Id);

                if (hasTickets)
                {
                    var flightToUpdate = await _flightRepository.GetByIdAsync(viewModel.Id);

                    flightToUpdate.DepartureTime = viewModel.DepartureTime;
                    flightToUpdate.ArrivalTime = viewModel.ArrivalTime;
                    flightToUpdate.BasePrice = viewModel.BasePrice;

                    await _flightRepository.UpdateAsync(flightToUpdate);

                    return RedirectToAction(nameof(Index));
                }


                var flight = _converterHelper.ToFlight(viewModel, false);
                await _flightRepository.UpdateAsync(flight);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Airplanes = _flightRepository.GetComboAirplanes();
            viewModel.Countries = _countryRepository.GetComboCountries();
            viewModel.DepartureAirports = _countryRepository.GetComboAirports(viewModel.DepartureCountryId);
            viewModel.ArrivalAirports = _countryRepository.GetComboAirports(viewModel.ArrivalCountryId);

            viewModel.HasTickets = await _ticketRepository.FlightHasTicketsAsync(viewModel.Id);

            return View(viewModel);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }
            var flight = await _flightRepository.GetFlightWithDetailsAsync(id.Value);

            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }
            return View(flight);
        }
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);

            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            flight.IsActive = false;

            await _flightRepository.UpdateAsync(flight);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetByIdAsync(id.Value);

            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            flight.IsActive = true;

            await _flightRepository.UpdateAsync(flight);

            return RedirectToAction(nameof(Index));
        }
    }
}
