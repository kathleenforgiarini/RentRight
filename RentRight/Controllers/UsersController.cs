using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Authorize]
    public class UsersController : Controller
    {
        private readonly RentRightContext _context;

        public UsersController(RentRightContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize (Policy = "RequireOwnerRole")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Create
        [Authorize(Policy = "RequireOwnerRole")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireOwnerRole")]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Password,Type,IsActive")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userFound = await _context.User.FindAsync(userId);
            if (userFound.Type != TypeUsers.Owner.ToString() && userId != id)
            {
                return RedirectToAction("AccessDenied", "Account");

            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Password,Type,IsActive")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userFound = await _context.User.FindAsync(userId);
                if (userFound.Type == TypeUsers.Owner.ToString())
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["SuccessMessage"] = "Account updated!";
                    return RedirectToAction("Index", "Properties");
                }
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userFound = await _context.User.FindAsync(userId);
            if (userFound.Type != TypeUsers.Owner.ToString() && userId != id)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId == id)
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        public IActionResult Search(string searchTerm)
        {
            var users = _context.User.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(p => p.FirstName.Contains(searchTerm) || p.LastName.Contains(searchTerm) || p.Email.Contains(searchTerm) || p.Type.Contains(searchTerm));
            }

            return View("Index", users.ToList());
        }
    }
}
