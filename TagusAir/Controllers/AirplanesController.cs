
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;
using TagusAir.Data;
using Microsoft.AspNetCore.Authorization;
using TagusAir.Helpers;
using TagusAir.Models;


[Authorize(Roles = "Admin")]
public class AirplanesController : Controller
{
    
    private readonly IAirplaneRepository _airplaneRepository;
    private readonly IImageHelper _imageHelper;

    public AirplanesController(
        IAirplaneRepository airplaneRepository, 
        IImageHelper imageHelper)
    {
        
        _airplaneRepository = airplaneRepository;
        _imageHelper = imageHelper;
    }

    // GET: AIRPLANES
    public IActionResult Index()    
    {
        return View(_airplaneRepository.GetAll());
    }

    // GET: AIRPLANES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var airplane = await _airplaneRepository.GetByIdAsync(id.Value);

        if (airplane == null)
        {
            return NotFound();
        }

        return View(airplane);
    }

    // GET: AIRPLANES/Create
    public IActionResult Create()
    {
        return View(new AirplaneViewModel());
    }

    // POST: AIRPLANES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AirplaneViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            string path = string.Empty;

            if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
            {
                path = await _imageHelper.UploadImageAsync(viewModel.ImageFile, "airplanes");
            }

            var airplane = ToAirplane(viewModel, path);

            await _airplaneRepository.CreateAsync(airplane);
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    // GET: AIRPLANES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var airplane = await _airplaneRepository.GetByIdAsync(id.Value);

        if (airplane == null)
        {
            return NotFound();
        }

        return View(ToAirplaneViewModel(airplane));
    }

    // POST: AIRPLANES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AirplaneViewModel viewModel)
    {
      
        if (ModelState.IsValid)
        {
            try
            {
                string path = viewModel.ImageUrl;

                if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(viewModel.ImageFile, "airplanes");
                }

                var airplane = ToAirplane(viewModel, path);

                await _airplaneRepository.UpdateAsync(airplane);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _airplaneRepository.ExistAsync(viewModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    // GET: AIRPLANES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var airplane = await _airplaneRepository.GetByIdAsync(id.Value);

        if (airplane == null)
        {
            return NotFound();
        }

        return View(airplane);
    }

    // POST: AIRPLANES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var airplane = await _airplaneRepository.GetByIdAsync(id.Value);

        if (airplane != null)
        {
            await _airplaneRepository.DeleteAsync(airplane);
        }

        
        return RedirectToAction(nameof(Index));
    }

    private Airplane ToAirplane(AirplaneViewModel model, string path)
    {
        return new Airplane
        {
            Id = model.Id,
            Brand = model.Brand,
            Model = model.Model,
            EconomySeats = model.EconomySeats,
            BusinessSeats = model.BusinessSeats,
            IsActive = model.IsActive,
            ImageUrl = path
        };
    }

    private AirplaneViewModel ToAirplaneViewModel(Airplane airplane)
    {
        return new AirplaneViewModel
        {
            Id = airplane.Id,
            Brand = airplane.Brand,
            Model = airplane.Model,
            EconomySeats = airplane.EconomySeats,
            BusinessSeats = airplane.BusinessSeats,
            IsActive = airplane.IsActive,
            ImageUrl = airplane.ImageUrl
        };
    }


}
