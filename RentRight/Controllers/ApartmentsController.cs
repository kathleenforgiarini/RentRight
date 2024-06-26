﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index(int propertyId)
        {
            var @property = await _context.Properties.FirstOrDefaultAsync(u => u.Id == propertyId);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userFound = await _context.Users.FindAsync(userId);

            var rentRightContext = await _context.Apartments.Include(a => a.Property)
                           .Where(a => a.PropertyId == propertyId).ToListAsync();

            if (userFound.Type == TypeUsers.Tenant.ToString())
            {
                rentRightContext = await _context.Apartments.Include(a => a.Property)
                           .Where(a => a.PropertyId == propertyId && a.Status == ApartmentStatus.Available.ToString())
                           .ToListAsync();
            }

            ViewBag.PropertyName = @property != null ? @property.Name : "Property not found";
            ViewBag.PropertyDescription = @property != null ? @property.Description : "Property not found";
            ViewBag.PropertyId = propertyId;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            return View(rentRightContext);
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
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Create([Bind("Number,Bedrooms,Bathrooms,Pets,Size,PropertyId,RentPrice,PhotoFile")] Apartment apartment)
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
                var apartmentFound = await _context.Apartments.FirstOrDefaultAsync(u => u.Number == apartment.Number && u.PropertyId == apartment.PropertyId);
                if (apartmentFound != null)
                {
                    TempData["ErrorMessage"] = "This apartment number already exists!";
                    return RedirectToAction("Index", new { propertyId = apartment.PropertyId });
                }

                _context.Add(apartment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Apartment created!";
                return RedirectToAction("Index", new { propertyId = apartment.PropertyId });
            }
            ViewBag.PropertyId = apartment.PropertyId;
            TempData["ErrorMessage"] = "Apartment was not created. Try again!";
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

            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Id", apartment.PropertyId);
            return View(apartment);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Bedrooms,Bathrooms,Pets,Size,PropertyId,RentPrice,Photo,PhotoFile,Status")] Apartment apartment)
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
                TempData["SuccessMessage"] = "Apartment updated!";
                return RedirectToAction("Index", new { propertyId = apartment.PropertyId });
            }
            ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Id", apartment.PropertyId);
            TempData["SuccessMessage"] = "Apartment was not updated. Try again!";
            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment != null)
            {
                var rentalFound = await _context.Rentals.FirstOrDefaultAsync(u => u.PropertyId == apartment.PropertyId && u.ApartmentNumber == apartment.Number);
                if (rentalFound != null)
                {
                    return StatusCode(500, "You canot delete this apartment, there is a rental related to it.");
                }

                _context.Apartments.Remove(apartment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Apartment deleted!";
                return StatusCode(200, apartment.PropertyId);
            }

            return StatusCode(500, "An error occurred while processing your request.");
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartments.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Search(string searchTerm, int propertyId, string searchBy, string searchFrom, string searchTo, string status)
        {
            var @property = await _context.Properties.FirstOrDefaultAsync(u => u.Id == propertyId);

            var rentRightContext = _context.Apartments.Include(a => a.Property)
                                                     .Where(a => a.PropertyId == propertyId);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userFound = await _context.Users.FindAsync(userId);

            if (userFound.Type == TypeUsers.Tenant.ToString())
            {
                rentRightContext = _context.Apartments.Include(a => a.Property)
                           .Where(a => a.PropertyId == propertyId && a.Status == ApartmentStatus.Available.ToString());
            }

            switch (searchBy)
            {
                case "all":
                    break;
                case "bed":
                    rentRightContext = rentRightContext.Where(p => (p.Bedrooms.ToString().Contains(searchTerm)));
                    break;
                case "bath":
                    rentRightContext = rentRightContext.Where(p => (p.Bathrooms.ToString().Contains(searchTerm)));
                    break;
                case "pet":
                    rentRightContext = rentRightContext.Where(p => (p.Pets));
                    break;
                case "size":
                    rentRightContext = rentRightContext.Where(p => (p.Size.ToString().Contains(searchTerm)));
                    break;
                case "price":
                    rentRightContext = rentRightContext.Where(p => (p.RentPrice >= Convert.ToDecimal(searchFrom) && p.RentPrice <= Convert.ToDecimal(searchTo)));
                    break;
                case "status":
                    rentRightContext = rentRightContext.Where(p => (p.Status == status));
                    break;
            }

            ViewBag.PropertyName = @property != null ? @property.Name : "Property not found";
            ViewBag.PropertyDescription = @property != null ? @property.Description : "Property not found";
            ViewBag.PropertyId = propertyId;
            return View("Index", await rentRightContext.ToListAsync());
        }

    }
}
