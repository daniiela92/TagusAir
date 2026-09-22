
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;
using TagusAir.Data;

public class AirplanesController : Controller
{
    
    private readonly IAirplaneRepository _airplaneRepository;

    public AirplanesController(IAirplaneRepository airplaneRepository)
    {
        
        _airplaneRepository = airplaneRepository;
    }

    // GET: AIRPLANES
    public async Task<IActionResult> Index()    
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
        return View();
    }

    // POST: AIRPLANES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Brand,Model,EconomySeats,BusinessSeats,IsActive,ImageUrl")] Airplane airplane)
    {
        if (ModelState.IsValid)
        {
            await _airplaneRepository.CreateAsync(airplane);
            return RedirectToAction(nameof(Index));
        }
        return View(airplane);
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

        return View(airplane);
    }

    // POST: AIRPLANES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Brand,Model,EconomySeats,BusinessSeats,IsActive,ImageUrl")] Airplane airplane)
    {
        if (id != airplane.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _airplaneRepository.UpdateAsync(airplane);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _airplaneRepository.ExistAsync(airplane.Id))
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
        return View(airplane);
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

  
}
