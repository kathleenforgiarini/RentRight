using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RentRight.Data;
using RentRight.Models;
using RentRight.Models.Enums;

namespace RentRight.Controllers
{
    [Authorize]
    public class ApartmentsController : Controller
    {
        private readonly RentRightContext _context;

        public ApartmentsController(RentRightContext context)
        {
            _context = context;
        }

        // GET: Apartments
        public async Task<IActionResult> Index(int propertyId)
        {
            var @property = await _context.Property.FirstOrDefaultAsync(u => u.Id == propertyId);
            var rentRightContext = await _context.Apartment.Include(a => a.Property)
                                                       .Where(a => a.PropertyId == propertyId).ToListAsync();

            ViewBag.PropertyName = @property != null ? @property.Name : "Property not found";
            ViewBag.PropertyDescription = @property != null ? @property.Description : "Property not found";
            ViewBag.PropertyId = propertyId;
            return View(rentRightContext);
        }

        // GET: Apartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment
                .Include(a => a.Property)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // GET: Apartments/Create
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public IActionResult Create(int propertyId)
        {
            ViewBag.PropertyId = propertyId;
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Bedrooms,Bathrooms,Pets,Size,PropertyId,RentPrice,PhotoFile,Status")] Apartment apartment)
        {
            if (apartment.PhotoFile != null && apartment.PhotoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await apartment.PhotoFile.CopyToAsync(memoryStream);
                    apartment.Photo = memoryStream.ToArray();
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(apartment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { propertyId = apartment.PropertyId });
            }
            ViewBag.PropertyId = apartment.PropertyId;
            return View(apartment);
        }

        // GET: Apartments/Edit/5
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            ViewData["PropertyId"] = new SelectList(_context.Property, "Id", "Id", apartment.PropertyId);
            return View(apartment);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Bedrooms,Bathrooms,Pets,Size,PropertyId,RentPrice,PhotoFile,Status")] Apartment apartment)
        {
            if (id != apartment.Id)
            {
                return NotFound();
            }

            if (apartment.PhotoFile != null && apartment.PhotoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await apartment.PhotoFile.CopyToAsync(memoryStream);
                    apartment.Photo = memoryStream.ToArray();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApartmentExists(apartment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewBag.PropertyId = apartment.PropertyId;
                return RedirectToAction("Index", new { propertyId = apartment.PropertyId });
            }
            ViewData["PropertyId"] = new SelectList(_context.Property, "Id", "Id", apartment.PropertyId);
            return View(apartment);
        }

        // GET: Apartments/Delete/5
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment
                .Include(a => a.Property)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartment.FindAsync(id);
            if (apartment != null)
            {
                _context.Apartment.Remove(apartment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { propertyId = apartment.PropertyId });
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartment.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Search(string searchTerm, int propertyId)
        {
            var @property = await _context.Property.FirstOrDefaultAsync(u => u.Id == propertyId);

            var rentRightContext = _context.Apartment.Include(a => a.Property)
                                                     .Where(a => a.PropertyId == propertyId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                rentRightContext = rentRightContext.Where(p => (p.Bedrooms.ToString().Contains(searchTerm) ||
                                                                p.Bathrooms.ToString().Contains(searchTerm) ||
                                                                p.Size.ToString().Contains(searchTerm) ||
                                                                p.RentPrice.ToString().Contains(searchTerm)));
            }

            ViewBag.PropertyName = @property != null ? @property.Name : "Property not found";
            ViewBag.PropertyDescription = @property != null ? @property.Description : "Property not found";
            ViewBag.PropertyId = propertyId;

            return View("Index", await rentRightContext.ToListAsync());
        }

    }
}
