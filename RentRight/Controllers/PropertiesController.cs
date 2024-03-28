using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentRight.Data;
using RentRight.Models;
using RentRight.Models.Enums;

namespace RentRight.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private readonly RentRightContext _context;

        public PropertiesController(RentRightContext context)
        {
            _context = context;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            return View(await _context.Property.ToListAsync());
        }


        // GET: Properties/Create
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Create()
        {
            List<User> owners = await _context.User.Where(u => u.Type == TypeUsers.Owner.ToString()).ToListAsync();
            List<User> managers = await _context.User.Where(u => u.Type == TypeUsers.Manager.ToString()).ToListAsync();
            ViewBag.Managers = managers;
            ViewBag.Owners = owners;
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Create([Bind("Name, Description, Street, StNumber, PostalCode, City, OwnerId, ManagerId, PhotoFile")] Property property)
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
                List<User> owners = await _context.User.Where(u => u.Type == TypeUsers.Owner.ToString()).ToListAsync();
                List<User> managers = await _context.User.Where(u => u.Type == TypeUsers.Manager.ToString()).ToListAsync();
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

            var @property = await _context.Property.FindAsync(id);
            if (@property == null)
            {
                return NotFound();
            }
            List<User> owners = await _context.User.Where(u => u.Type == TypeUsers.Owner.ToString()).ToListAsync();
            List<User> managers = await _context.User.Where(u => u.Type == TypeUsers.Manager.ToString()).ToListAsync();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Street,StNumber,PostalCode,City,OwnerId,ManagerId,Photo, PhotoFile")] Property @property)
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
            List<User> owners = await _context.User.Where(u => u.Type == TypeUsers.Owner.ToString()).ToListAsync();
            List<User> managers = await _context.User.Where(u => u.Type == TypeUsers.Manager.ToString()).ToListAsync();
            ViewBag.Managers = managers;
            ViewBag.Owners = owners;
            return View(@property);
        }

        // GET: Properties/Delete/5
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Property
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireOwnerOrManagerRole")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @property = await _context.Property.FindAsync(id);
            if (@property != null)
            {
                _context.Property.Remove(@property);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Property deleted!";
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
            return _context.Property.Any(e => e.Id == id);
        }

        public IActionResult Search(string searchTerm)
        {
            var properties = _context.Property.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                properties = properties.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm) || p.City.Contains(searchTerm));
            }

            return View("Index", properties.ToList());
        }
    }
}
