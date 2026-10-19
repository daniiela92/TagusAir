using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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

        public TicketsController(
            ITicketRepository ticketRepository,
            IFlightRepository flightRepository,
            IUserHelper userHelper)
        {
            _ticketRepository = ticketRepository;
            _flightRepository = flightRepository;
            _userHelper = userHelper;
        }

        public async Task<ActionResult> Buy(int? id)
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

            var viewModel = new BuyTicketViewModel
            {
                FlightId = flight.Id,
                Flight = flight,
                Seats = await GetSeatsComboAsync(flight.Id)

            };

            return View(viewModel);



        }


        private async Task<IEnumerable<SelectListItem>> GetSeatsComboAsync(int flightId)
        {

            var seats = await _ticketRepository.GetAvailableSeatsAsync(flightId);


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
                        await _ticketRepository.BuyTicketsAsync(viewModel.FlightId, viewModel.SeatId, user.Id);
                        return RedirectToAction(nameof(MyTickets));
                    }
                }
                
            }

            viewModel.Flight = await _flightRepository.GetFlightWithDetailsAsync(viewModel.FlightId);
            viewModel.Seats = await GetSeatsComboAsync(viewModel.FlightId);
            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> MyTickets()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var list = await _ticketRepository.GetTicketsByUserAsync(user.Id);
            return View(list);
        }

        private async Task<User?> GetOrCreateUserAsync(BuyTicketViewModel viewModel)
        {

            if (this.User.Identity.IsAuthenticated)
            {

                return await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            }

            if (string.IsNullOrEmpty(viewModel.FirstName) || string.IsNullOrEmpty(viewModel.LastName) || string.IsNullOrEmpty(viewModel.Email))
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
            };



            var result = await _userHelper.AddUserAsync(createdUser, generatedPassword);

            if (result != IdentityResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Could not create the user account");
                return null;
            }

            await _userHelper.AddUserToRoleAsync(createdUser, "Customer");

            var loginViewModel = new LoginViewModel
            {
                Username = viewModel.Email,
                Password = generatedPassword,
                RememberMe = false,

            };

            await _userHelper.LoginAsync(loginViewModel);

            return createdUser;

        }
    }
}
