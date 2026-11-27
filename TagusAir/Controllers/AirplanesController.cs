
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
    private readonly IMailHelper _mailHelper;

    public AirplanesController(
        IAirplaneRepository airplaneRepository, 
        IImageHelper imageHelper,
        IConverterHelper converterHelper,
        IMailHelper mailHelper)
    {
        
        _airplaneRepository = airplaneRepository;
        _imageHelper = imageHelper;
       _converterHelper = converterHelper;
        _mailHelper = mailHelper;
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
            return new NotFoundViewResult("AirplaneNotFound");
        }

        var airplane = await _airplaneRepository.GetByIdAsync(id.Value);

        if (airplane == null)
        {
            return new NotFoundViewResult("AirplaneNotFound");
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
            if (viewModel.ImageFile == null || viewModel.ImageFile.Length == 0)
            {
                ModelState.AddModelError(nameof(viewModel.ImageFile), "An airplane must have a photo.");
                return View(viewModel);
            }

            string path = await _imageHelper.UploadImageAsync(viewModel.ImageFile, "airplanes");

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
            return new NotFoundViewResult("AirplaneNotFound");
        }

        var airplane = await _airplaneRepository.GetByIdAsync(id.Value);

        if (airplane == null)
        {
            return new NotFoundViewResult("AirplaneNotFound");
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
            if (string.IsNullOrEmpty(viewModel.ImageUrl)
                && (viewModel.ImageFile == null || viewModel.ImageFile.Length == 0))
            {
                ModelState.AddModelError(nameof(viewModel.ImageFile), "An airplane must have a photo.");
                return View(viewModel);
            }

            try
            {
                string path = viewModel.ImageUrl;

                if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(viewModel.ImageFile, "airplanes");
                }

                var airplane = _converterHelper.ToAirplane(viewModel, path, false);

                var reseatedTickets = await _airplaneRepository.UpdateWithSeatsAsync(airplane);

                foreach (var ticket in reseatedTickets)
                {
                    var flightNumber = ticket.Flight.FlightNumber;
                    var route = $"{ticket.Flight.DepartureAirport.City} to {ticket.Flight.ArrivalAirport.City}";
                    var departureDate = ticket.Flight.DepartureTime.ToString("dd/MM/yyyy");
                    var departureTime = ticket.Flight.DepartureTime.ToString("HH:mm");

                    string subject;
                    string body;

                    if (ticket.IsCanceled)
                    {
                        subject = $"Your booking on flight {flightNumber} has been cancelled";

                        body = $@"
                         <p>Dear {ticket.User.FirstName},</p>

                         <p>We had to change the aircraft operating flight <strong>{flightNumber}</strong>,
                         {route}, departing on {departureDate} at {departureTime}.</p>

                        <p>The new aircraft has fewer seats in {ticket.Seat.Class} class, and we were not
                        able to keep a seat for you. Your ticket has been cancelled.</p>

                        <p>We are truly sorry for the inconvenience. Please get in touch with us and we
                        will find you a seat on another flight or arrange your refund, whichever suits
                        you best.</p>

                         <p>Kind regards,<br />The TagusAir team</p>";
                    }
                    else
                    {
                        subject = $"Your seat on flight {flightNumber} has changed";

                        body = $@"
                        <p>Dear {ticket.User.FirstName},</p>

                        <p>We had to change the aircraft operating flight <strong>{flightNumber}</strong>,
                        {route}, departing on {departureDate} at {departureTime}.</p>

                        <p>The new aircraft has a different seat layout, so we moved you to
                        seat <strong>{ticket.Seat.Code}</strong>, still in {ticket.Seat.Class} class.
                        Everything else about your booking stays exactly the same.</p>

                        <p>You can see your updated boarding pass at any time under My Tickets.</p>

                        <p>We are sorry for the change, and we look forward to welcoming you on board.</p>

                        <p>Kind regards,<br />The TagusAir team</p>";
                    }

                    _mailHelper.SendEmail(ticket.User.Email, subject, body);
                }
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
            return new NotFoundViewResult("AirplaneNotFound");
        }

        var airplane = await _airplaneRepository.GetByIdAsync(id.Value);

        if (airplane == null)
        {
            return new NotFoundViewResult("AirplaneNotFound");
        }
         

        return View(airplane);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null)
        {
            return new NotFoundViewResult("AirplaneNotFound");
        }

        var airplane = await _airplaneRepository.GetByIdAsync(id.Value);

        if (airplane == null)
        {
            return new NotFoundViewResult("AirplaneNotFound");
        }

        var airplaneHasFlights = await _airplaneRepository.AirplaneHasFlightsAsync(id.Value);

        if (airplaneHasFlights)
        {
            ModelState.AddModelError(string.Empty, "This airplane cannot be deleted because it is assigned to flights. Delete those flights first.");
            return View(airplane);
        }

        await _airplaneRepository.DeleteWithSeatsAsync(airplane);

        return RedirectToAction(nameof(Index));
    }
}
