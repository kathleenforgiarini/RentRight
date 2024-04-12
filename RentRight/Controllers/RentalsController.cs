using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentRight.Data;
using RentRight.Models;
using RentRight.Models.Enums;

namespace RentRight.Controllers
{
    [Authorize(Policy = "RequireOwnerOrManagerRole")]
    public class RentalsController : Controller
    {
        private readonly RentRightContext _context;

        public RentalsController(RentRightContext context)
        {
            _context = context;
        }

        // GET: Rentals
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var rentRightContext = _context.Rentals.Include(r => r.Property).Include(r => r.Tenant);
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            return View(await rentRightContext.ToListAsync());
        }

        // GET: Rentals/Create
        public async Task<IActionResult> Create()
        {
            List<User> tenants = await _context.Users.Where(u => u.Type == TypeUsers.Tenant.ToString() && u.IsActive).ToListAsync();
            List<Property> properties = await _context.Properties.ToListAsync();

            ViewBag.Tenants = tenants;
            ViewBag.Properties = properties;
            return View();
        }

        [HttpGet]
        public IActionResult GetApartmentsByPropertyId(int propertyId)
        {
            var apartments = _context.Apartments.Where(a => a.PropertyId == propertyId && a.Status == ApartmentStatus.Available.ToString()).ToList();
            return Json(apartments);
        }

        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyId,ApartmentNumber,TenantId,RentedDate,Months")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                if (rental.RentedDate == null)
                {
                    rental.RentedDate = DateTime.Now;
                }
                var rentalFound = await _context.Rentals.FirstOrDefaultAsync(u => u.PropertyId == rental.PropertyId && u.ApartmentNumber == rental.ApartmentNumber);
                if (rentalFound != null)
                {
                    TempData["ErrorMessage"] = "This rental is already registered!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Add(rental);
                await _context.SaveChangesAsync();

                var apartmentFound = await _context.Apartments.FirstOrDefaultAsync(u => u.PropertyId == rental.PropertyId && u.Number == rental.ApartmentNumber);
                if (apartmentFound != null)
                {
                    apartmentFound.Status = ApartmentStatus.Rented.ToString();
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Rental inserted!";
                return RedirectToAction(nameof(Index));
            }
            List<User> tenants = await _context.Users.Where(u => u.Type == TypeUsers.Tenant.ToString() && u.IsActive).ToListAsync();
            ViewBag.Tenants = tenants;
            List<Apartment> apartments = await _context.Apartments.Where(u => u.Status == ApartmentStatus.Rented.ToString()).ToListAsync();
            ViewBag.Apartments = apartments;
            TempData["ErrorMessage"] = "Rental was not inserted. Try again!";
            return View(rental);
        }

        // GET: Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Property)
                .Include(r => r.Tenant)
                .FirstOrDefaultAsync(r => r.Id == id);
            
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string rentedDate, int months)
        {
            var rental = await _context.Rentals.FirstOrDefaultAsync(r => r.Id == id);
            if (rental == null)
            {
                return NotFound();
            }
            
            try
            {
                rental.RentedDate = DateTime.Parse(rentedDate);
                rental.Months = months;
                _context.Update(rental);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Rental updated!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Rental was not updated. Try again!";
                return RedirectToAction(nameof(Index));
            }

        }

        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Property)
                .Include(r => r.Tenant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
                var apartmentFound = await _context.Apartments.FirstOrDefaultAsync(u => u.PropertyId == rental.PropertyId && u.Number == rental.ApartmentNumber);
                if (apartmentFound != null)
                {
                    apartmentFound.Status = ApartmentStatus.Available.ToString();
                    await _context.SaveChangesAsync();
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Rental deleted!";
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.Id == id);
        }
    }
}
