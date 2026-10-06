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

        public FlightsController(
            IFlightRepository flightRepository,
            IConverterHelper converterHelper
            )
        {
            _flightRepository = flightRepository;
            _converterHelper = converterHelper;
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
                Airplanes = _flightRepository.GetComboAirplanes(),
                DepartureAirports = _flightRepository.GetComboAirports(),
                ArrivalAirports = _flightRepository.GetComboAirports()

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
            viewModel.DepartureAirports = _flightRepository.GetComboAirports();
            viewModel.ArrivalAirports = _flightRepository.GetComboAirports();

            return View(viewModel);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetByIdAsync(id.Value);

            if (flight == null)
            {
                return NotFound();
            }

            var viewModel = _converterHelper.ToFlightViewModel(flight);

            viewModel.Airplanes = _flightRepository.GetComboAirplanes();
            viewModel.DepartureAirports = _flightRepository.GetComboAirports();
            viewModel.ArrivalAirports = _flightRepository.GetComboAirports();

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
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetByIdAsync(id.Value);

            if (flight != null)
            {
                await _flightRepository.DeleteAsync(flight);
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
