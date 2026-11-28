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
        private readonly IMailHelper _mailHelper;

        public FlightsController(
            IFlightRepository flightRepository,
            IConverterHelper converterHelper,
            ICountryRepository countryRepository,
            ITicketRepository ticketRepository,
            IMailHelper mailHelper
            )
        {
            _flightRepository = flightRepository;
            _converterHelper = converterHelper;
            _countryRepository = countryRepository;
            _ticketRepository = ticketRepository;
            _mailHelper = mailHelper;
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
                    var flightToUpdate = await _flightRepository.GetFlightWithDetailsAsync(viewModel.Id);

                    var oldDepartureTime = flightToUpdate.DepartureTime;

                    flightToUpdate.DepartureTime = viewModel.DepartureTime;
                    flightToUpdate.ArrivalTime = viewModel.ArrivalTime;
                    flightToUpdate.BasePrice = viewModel.BasePrice;

                    await _flightRepository.UpdateAsync(flightToUpdate);

                    if (oldDepartureTime != flightToUpdate.DepartureTime)
                    {
                        var tickets = await _ticketRepository.GetTicketsByFlightAsync(flightToUpdate.Id);

                        foreach (var ticket in tickets)
                        {
                            var subject = $"New departure time for flight {flightToUpdate.FlightNumber}";

                            var body = $@"
                        <p>Dear {ticket.User.FirstName},</p>

                        <p>The departure time of flight <strong>{flightToUpdate.FlightNumber}</strong>,
                        {flightToUpdate.DepartureAirport.City} to {flightToUpdate.ArrivalAirport.City},
                        has changed.</p>

                        <p>It now departs on <strong>{flightToUpdate.DepartureTime:dd/MM/yyyy}</strong> at
                        <strong>{flightToUpdate.DepartureTime:HH:mm}</strong>, instead of
                        {oldDepartureTime:dd/MM/yyyy} at {oldDepartureTime:HH:mm}.</p>

                        <p>Your seat, <strong>{ticket.Seat.Code}</strong>, stays the same.</p>

                        <p>Kind regards,<br />The TagusAir team</p>";

                            _mailHelper.SendEmail(ticket.User.Email, subject, body);
                        }
                    }

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
            var flight = await _flightRepository.GetFlightWithDetailsAsync(id);

            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            flight.IsActive = false;
            await _flightRepository.UpdateAsync(flight);

            var tickets = await _ticketRepository.GetTicketsByFlightAsync(flight.Id);

            foreach (var ticket in tickets)
            {
                var subject = $"Flight {flight.FlightNumber} has been cancelled";

                var body = $@"
            <p>Dear {ticket.User.FirstName},</p>

            <p>We are very sorry to tell you that flight <strong>{flight.FlightNumber}</strong>,
            {flight.DepartureAirport.City} to {flight.ArrivalAirport.City}, departing on
            {flight.DepartureTime:dd/MM/yyyy} at {flight.DepartureTime:HH:mm}, will not operate.</p>

            <p>Your seat was <strong>{ticket.Seat.Code}</strong>. Please get in touch with us and we
            will find you a place on another flight or arrange your refund, whichever suits you best.</p>

            <p>Kind regards,<br />The TagusAir team</p>";

                _mailHelper.SendEmail(ticket.User.Email, subject, body);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("Flights/GetAirportsAsync")]
        public async Task<JsonResult> GetAirportsAsync(int countryId)
        {
            var country = await _countryRepository.GetCountryWithAirportsAsync(countryId);

            return Json(country.Airports
                .Where(a => a.IsActive)
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
