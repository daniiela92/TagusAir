using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using TagusAir.Data;
using TagusAir.Data.Entities;
using TagusAir.Helpers;
using TagusAir.Models;

namespace TagusAir.Controllers
{
    
    public class TicketsController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IPdfHelper _pdfHelper;

        public TicketsController(
            ITicketRepository ticketRepository,
            IFlightRepository flightRepository,
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IPdfHelper pdfHelper)
        {
            _ticketRepository = ticketRepository;
            _flightRepository = flightRepository;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _pdfHelper = pdfHelper;
        }

        public async Task<ActionResult> Buy(int? id)
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

            var viewModel = new BuyTicketViewModel
            {
                FlightId = flight.Id,
                Flight = flight,
                Seats = await GetSeatsComboAsync(flight.Id)

            };

            return View(viewModel);



        }


        private async Task<IEnumerable<SelectListItem>> GetSeatsComboAsync(int flightId, SeatClass? seatClass = null)
        {
            var seats = await _ticketRepository.GetAvailableSeatsAsync(flightId);

            if (seatClass != null)
            {
                seats = seats.Where(s => s.Class == seatClass).ToList();
            }

            var list = seats
                .Select(s => new SelectListItem
                {
                    Text = $"{s.Code} - {s.Class}",
                    Value = s.Id.ToString()
                })
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a seat...)",
                Value = "0"
            });

            return list;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(BuyTicketViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var seatAvailable = await _ticketRepository.IsSeatAvailableAsync(viewModel.FlightId, viewModel.SeatId);

                if (viewModel.SeatId == 0)
                {
                    ModelState.AddModelError(string.Empty, "You must select a seat");
                }
                else if (seatAvailable == false)
                {
                    ModelState.AddModelError(string.Empty, "Sorry, this seat has been taken.");
                }
                else
                {
                    var user = await GetOrCreateUserAsync(viewModel);

                    if (user != null)
                    {
                        var ticket = await _ticketRepository.BuyTicketsAsync(viewModel.FlightId, viewModel.SeatId, user.Id);

                        if (ticket == null)
                        {
                            ModelState.AddModelError(string.Empty, "This flight or seat is no longer available.");
                        }
                        else
                        {
                            var tickets = await _ticketRepository.GetTicketsByUserAsync(user.Id);
                            var fullTicket = tickets.FirstOrDefault(t => t.Id == ticket.Id);

                            if (fullTicket != null)
                            {
                                var passengerName = $"{user.FirstName} {user.LastName}";
                                var pdf = _pdfHelper.GenerateTicketPdf(fullTicket, passengerName);

                                var subject = $"Your ticket for flight {fullTicket.Flight.FlightNumber}";

                                var body = $@"
                            <p>Dear {user.FirstName},</p>

                            <p>Thank you for flying with us. Your boarding pass for flight
                            <strong>{fullTicket.Flight.FlightNumber}</strong>,
                            {fullTicket.Flight.DepartureAirport.City} to {fullTicket.Flight.ArrivalAirport.City}
                            on {fullTicket.Flight.DepartureTime:dd/MM/yyyy} at
                            {fullTicket.Flight.DepartureTime:HH:mm}, is attached to this email.</p>

                            <p>Your seat is <strong>{fullTicket.Seat.Code}</strong>, in
                            {fullTicket.Seat.Class} class.</p>

                            <p>Have a good trip,<br />The TagusAir team</p>";

                                _mailHelper.SendEmail(user.Email, subject, body, pdf, $"TagusAir-{ticket.Id}.pdf");
                            }

                            return RedirectToAction(nameof(Details), new { id = ticket.Id });
                        }
                    }
                }
            }

            viewModel.Flight = await _flightRepository.GetFlightWithDetailsAsync(viewModel.FlightId);
            viewModel.Seats = await GetSeatsComboAsync(viewModel.FlightId);
            return View(viewModel);
        }

        [Authorize(Roles = "Employee, Customer")]
        public async Task<IActionResult> MyTickets(string search)
        {
            var list = await _ticketRepository.GetTicketsAsync(this.User.Identity.Name);
            if(!string.IsNullOrEmpty(search))
            {
                list = list.Where(t => (t.User.FirstName + " " + t.User.LastName).Contains(search));
            }

            ViewBag.Search = search;

            return View(list);
        }

        private async Task<User?> GetOrCreateUserAsync(BuyTicketViewModel viewModel)
        {

            if (this.User.Identity.IsAuthenticated)
            {

                return await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            }

            if (string.IsNullOrEmpty(viewModel.FirstName)
               || string.IsNullOrEmpty(viewModel.LastName)
               || string.IsNullOrEmpty(viewModel.Email))
            {
                ModelState.AddModelError(string.Empty, "Please fill in your name and email");
                return null;
            }

            var userExists = await _userHelper.GetUserByEmailAsync(viewModel.Email);

            if (userExists != null)
            {
                ModelState.AddModelError(string.Empty, "This email is already registered. Please log in to buy your ticket.");
                return null;
            }


            var generatedPassword = Guid.NewGuid().ToString();

            User createdUser = new User
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                UserName = viewModel.Email,
                Passport = viewModel.Passport,
                PhoneNumber = viewModel.PhoneNumber,
            };



            var result = await _userHelper.AddUserAsync(createdUser, generatedPassword);

            if (result != IdentityResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Could not create the user account");
                return null;
            }

            await _userHelper.AddUserToRoleAsync(createdUser, "Customer");

            var passwordToken = await _userHelper.GeneratePasswordResetTokenAsync(createdUser);

            var link = this.Url.Action(
                "ResetPassword",
                "Account",
                new { token = passwordToken }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendEmail(createdUser.Email, "Welcome to TagusAir - set your password",
                $@"
                <p>Dear {createdUser.FirstName},</p>

                <p>We created an account for you so you can manage your booking later.
                To choose your own password, use the link below.</p>

                <p><a href=""{link}"">Set my password</a></p>

                <p>Kind regards,<br />The TagusAir team</p>");

            var loginViewModel = new LoginViewModel
            {
                Username = viewModel.Email,
                Password = generatedPassword,
                RememberMe = false,

            };

            await _userHelper.LoginAsync(loginViewModel);

            return createdUser;

        }

        
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            var tickets = await _ticketRepository.GetTicketsByUserAsync(user.Id);

            var ticket = tickets.FirstOrDefault(t => t.Id == id);

            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            return View(ticket);
        }

        [HttpPost]
        [ActionName("Cancel")]
        [Authorize]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            var tickets = await _ticketRepository.GetTicketsByUserAsync(user.Id);

            var ticket = tickets.FirstOrDefault(t => t.Id == id);

            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            ticket.IsCanceled = true;
            await _ticketRepository.UpdateAsync(ticket);

            return RedirectToAction(nameof(MyTickets));

        }

        [Authorize(Roles = "Employee,Customer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            var tickets = await _ticketRepository.GetTicketsAsync(this.User.Identity.Name);
            var ticket = await tickets.FirstOrDefaultAsync(t => t.Id == id.Value);

            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            return View(ticket);
        }
        [Authorize(Roles = "Employee,Customer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            var tickets = await _ticketRepository.GetTicketsAsync(this.User.Identity.Name);
            var ticket = await tickets.FirstOrDefaultAsync(t => t.Id == id.Value);

            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            if (ticket.IsCanceled || ticket.Flight.DepartureTime <= DateTime.Now)
            {
                return RedirectToAction(nameof(Details), new { id = ticket.Id });
            }

            var viewModel = new EditTicketViewModel
            {
                Id = ticket.Id,
                Flight = ticket.Flight,
                CurrentSeat = ticket.Seat.Code,
                Seats = await GetSeatsComboAsync(ticket.FlightId, ticket.Seat.Class)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditTicketViewModel viewModel)
        {
            var tickets = await _ticketRepository.GetTicketsAsync(this.User.Identity.Name);
            var ticket = await tickets.FirstOrDefaultAsync(t => t.Id == viewModel.Id);

            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            if (ticket.IsCanceled || ticket.Flight.DepartureTime <= DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "This ticket cannot be changed after the flight has started.");
            }
            else if (viewModel.SeatId == 0)
            {
                ModelState.AddModelError(string.Empty, "You must select a seat.");
            }
            else
            {
                var availableSeats = await _ticketRepository.GetAvailableSeatsAsync(ticket.FlightId);
                var newSeat = availableSeats.FirstOrDefault(s => s.Id == viewModel.SeatId);

                if (newSeat == null)
                {
                    ModelState.AddModelError(string.Empty, "Sorry, this seat has been taken.");
                }
                else if (newSeat.Class != ticket.Seat.Class)
                {
                    ModelState.AddModelError(string.Empty, "You can only move to a seat in the same class.");
                }
                else
                {
                    ticket.SeatId = newSeat.Id;
                    await _ticketRepository.UpdateAsync(ticket);

                    return RedirectToAction(nameof(Details), new { id = ticket.Id });
                }
            }

            viewModel.Flight = ticket.Flight;
            viewModel.CurrentSeat = ticket.Seat.Code;
            viewModel.Seats = await GetSeatsComboAsync(ticket.FlightId, ticket.Seat.Class);

            return View(viewModel);
        }

    }
}
