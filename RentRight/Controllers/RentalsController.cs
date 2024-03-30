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
            var rentRightContext = _context.Rental.Include(r => r.Apartment).Include(r => r.Tenant).Include(r => r.Apartment.Property);
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            return View(await rentRightContext.ToListAsync());
        }

        // GET: Rentals/Create
        public async Task<IActionResult> Create()
        {
            List<User> tenants = await _context.User.Where(u => u.Type == TypeUsers.Tenant.ToString() && u.IsActive).ToListAsync();
            ViewBag.Tenants = tenants;

            List<Apartment> apartments = await _context.Apartment.Where(u => u.Status == ApartmentStatus.Rented.ToString()).ToListAsync();
            ViewBag.Apartments = apartments;
            return View();
        }

        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApartmentId,TenantId,RentedDate,Months")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                if (rental.RentedDate == null)
                {
                    rental.RentedDate = DateTime.UtcNow.Date;
                }
                _context.Add(rental);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Rental inserted!";
                return RedirectToAction(nameof(Index));
            }
            List<User> tenants = await _context.User.Where(u => u.Type == TypeUsers.Tenant.ToString() && u.IsActive).ToListAsync();
            ViewBag.Tenants = tenants;
            List<Apartment> apartments = await _context.Apartment.Where(u => u.Status == ApartmentStatus.Rented.ToString()).ToListAsync();
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

            var rental = await _context.Rental.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            List<User> tenants = await _context.User.Where(u => u.Type == TypeUsers.Tenant.ToString() && u.IsActive).ToListAsync();
            ViewBag.Tenants = tenants;

            List<Apartment> apartments = await _context.Apartment.Where(u => u.Status == ApartmentStatus.Rented.ToString()).ToListAsync();
            ViewBag.Apartments = apartments;
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApartmentId,TenantId,RentedDate,Months")] Rental rental)
        {
            if (id != rental.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    TempData["SuccessMessage"] = "Rental updated!";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.Id))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartment, "Id", "Id", rental.ApartmentId);
            ViewData["TenantId"] = new SelectList(_context.User, "Id", "Id", rental.TenantId);
            TempData["ErrorMessage"] = "Rental was not updated. Try again!";
            return View(rental);
        }

        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental
                .Include(r => r.Apartment)
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
            var rental = await _context.Rental.FindAsync(id);
            if (rental != null)
            {
                _context.Rental.Remove(rental);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Rental deleted!";
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rental.Any(e => e.Id == id);
        }
    }
}
