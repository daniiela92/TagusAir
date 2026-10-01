using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TagusAir.Data;
using TagusAir.Data.Entities;
using TagusAir.Models;

namespace TagusAir.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly ICountryRepository _countryRepository;

        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public IActionResult Index()
        {
            return View(_countryRepository.GetCountriesWithAirports());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetCountryWithAirportsAsync(id.Value);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.CreateAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetByIdAsync(id.Value);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.UpdateAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetByIdAsync(id.Value);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);

            if (country != null)
            {
                await _countryRepository.DeleteAsync(country);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddAirport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetByIdAsync(id.Value);

            if (country == null)
            {
                return NotFound();
            }

            var model = new AirportViewModel { CountryId = country.Id };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddAirport(AirportViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.AddAirportAsync(model);
                return RedirectToAction(nameof(Details), new { id = model.CountryId });
            }

            return View(model);
        }

        public async Task<IActionResult> EditAirport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _countryRepository.GetAirportAsync(id.Value);

            if (airport == null)
            {
                return NotFound();
            }

            return View(airport);
        }

        [HttpPost]
        public async Task<IActionResult> EditAirport(Airport airport)
        {
            if (ModelState.IsValid)
            {
                var countryId = await _countryRepository.UpdateAirportAsync(airport);

                if (countryId != 0)
                {
                    return RedirectToAction(nameof(Details), new { id = countryId });
                }
            }

            return View(airport);
        }

        public async Task<IActionResult> DeleteAirport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _countryRepository.GetAirportAsync(id.Value);

            if (airport == null)
            {
                return NotFound();
            }

            var countryId = await _countryRepository.DeleteAirportAsync(airport);

            return RedirectToAction(nameof(Details), new { id = countryId });
        }
    }
}

