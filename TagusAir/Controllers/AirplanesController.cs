
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;
using TagusAir.Data;

public class AirplanesController : Controller
{
    private readonly DataContext _context;

    public AirplanesController(DataContext context)
    {
        _context = context;
    }

    // GET: AIRPLANES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Airplanes.ToListAsync());
    }

    // GET: AIRPLANES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var airplane = await _context.Airplanes
            .FirstOrDefaultAsync(m => m.Id == id);
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
            _context.Add(airplane);
            await _context.SaveChangesAsync();
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

        var airplane = await _context.Airplanes.FindAsync(id);
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
                _context.Update(airplane);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirplaneExists(airplane.Id))
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

        var airplane = await _context.Airplanes
            .FirstOrDefaultAsync(m => m.Id == id);
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
        var airplane = await _context.Airplanes.FindAsync(id);
        if (airplane != null)
        {
            _context.Airplanes.Remove(airplane);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AirplaneExists(int? id)
    {
        return _context.Airplanes.Any(e => e.Id == id);
    }
}
