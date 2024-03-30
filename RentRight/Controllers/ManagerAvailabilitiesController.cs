using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentRight.Data;
using RentRight.Models;

namespace RentRight.Controllers
{
    [Authorize(Policy = "RequireManagerRole")]
    public class ManagerAvailabilitiesController : Controller
    {
        private readonly RentRightContext _context;

        public ManagerAvailabilitiesController(RentRightContext context)
        {
            _context = context;
        }

        // GET: ManagerAvailabilities
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var rentRightContext = _context.ManagerAvailability.Include(m => m.Manager);
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            return View(await rentRightContext.ToListAsync());
        }

        // POST: ManagerAvailabilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string day, string time)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ManagerAvailability managerAvailability = new ManagerAvailability();
            managerAvailability.DayOfTheWeek = day;
            TimeSpan timespan = TimeSpan.ParseExact(time, @"hh\:mm", CultureInfo.InvariantCulture);
            managerAvailability.Time = timespan;
            managerAvailability.ManagerId = userId;

            if (ModelState.IsValid)
            {
                var availabilityFound = await _context.ManagerAvailability.FirstOrDefaultAsync(u => u.DayOfTheWeek == day && u.Time == timespan && u.ManagerId == userId);
                if (availabilityFound != null)
                {
                    TempData["ErrorMessage"] = "This slot is already registered!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Add(managerAvailability);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Slot created!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ManagerId"] = new SelectList(_context.User, "Id", "Id", managerAvailability.ManagerId);
            TempData["ErrorMessage"] = "Slot was not created. Try again!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var managerAvailability = await _context.ManagerAvailability.FindAsync(id);
            if (managerAvailability != null)
            {
                _context.ManagerAvailability.Remove(managerAvailability);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Slot deleted!";
                return StatusCode(200);
            }
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
