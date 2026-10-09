
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
    private readonly IConverterHelper _converterHelper;

    public AirplanesController(
        IAirplaneRepository airplaneRepository, 
        IImageHelper imageHelper,
        IConverterHelper converterHelper)
    {
        
        _airplaneRepository = airplaneRepository;
        _imageHelper = imageHelper;
       _converterHelper = converterHelper;
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

            var airplane = _converterHelper.ToAirplane(viewModel, path, true);

            await _airplaneRepository.CreateWithSeatsAsync(airplane);
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

        return View(_converterHelper.ToAirplaneViewModel(airplane));
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

                var airplane = _converterHelper.ToAirplane(viewModel, path, false);

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
            await _airplaneRepository.DeleteWithSeatsAsync(airplane);
        }

        
        return RedirectToAction(nameof(Index));
    }

   

}
