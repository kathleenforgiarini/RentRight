using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class PropertiesController : Controller
    {
        private readonly RentRightContext _context;
        private List<User> owners;
        private List<User> managers;

        public PropertiesController(RentRightContext context)
        {
            _context = context;
            this.owners = _context.Users.Where(u => u.Type == TypeUsers.Owner.ToString() && u.IsActive).ToList();
            this.managers = _context.Users.Where(u => u.Type == TypeUsers.Manager.ToString() && u.IsActive).ToList();

        }

        // GET: Properties
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            return View(await _context.Properties.ToListAsync());
        }

        // GET: Properties/Create
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Managers = this.managers;
            ViewBag.Owners = this.owners;
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Create([Bind("Name, Description, Street, StNumber, PostalCode, City, OwnerId, ManagerId, PhotoFile")] Models.Property property)
        {
            if (property.PhotoFile != null && property.PhotoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await property.PhotoFile.CopyToAsync(memoryStream);
                    property.Photo = memoryStream.ToArray();
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Managers = this.managers;
                ViewBag.Owners = this.owners;
                ViewBag.Managers = managers;
                ViewBag.Owners = owners;
                return View(property);
            }
            else
            {
                _context.Add(property);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Property created!";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Properties/Edit/5
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties.FindAsync(id);
            if (@property == null)
            {
                return NotFound();
            }
            ViewBag.Managers = this.managers;
            ViewBag.Owners = this.owners;
            ViewBag.Managers = managers;
            ViewBag.Owners = owners;

            return View(@property);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Street,StNumber,PostalCode,City,OwnerId,ManagerId,Photo,PhotoFile")] Models.Property @property)
        {
            if (id != @property.Id)
            {
                return NotFound();
            }

            if (property.PhotoFile != null && property.PhotoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await property.PhotoFile.CopyToAsync(memoryStream);
                    property.Photo = memoryStream.ToArray();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@property);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Property updated!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(@property.Id))
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
            ViewBag.Managers = this.managers;
            ViewBag.Owners = this.owners;
            ViewBag.Managers = managers;
            ViewBag.Owners = owners;
            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @property = await _context.Properties.FindAsync(id);
            if (@property != null)
            {
                var apartments = await _context.Apartments.Where(a => a.PropertyId == id).ToListAsync();
                if (apartments.Count > 0)
                {
                    return StatusCode(500, "You canot delete this property, there are apartments related to it.");
                }
                try
                {
                    _context.Properties.Remove(@property);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Property deleted!";
                    return StatusCode(200);
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(500, "You canot delete this property, there are rentals related to it!");
                }

            }
            return StatusCode(500, "An error occurred while processing your request.");
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }

        public IActionResult Search(string searchTerm)
        {
            var properties = _context.Properties.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                properties = properties.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm) || p.City.Contains(searchTerm));
            }

            return View("Index", properties.ToList());
        }
    }
}
