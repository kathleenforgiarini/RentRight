﻿using System;
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            return View(await _context.Users.ToListAsync());
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
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = "This e-mail already exists!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User created!";
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
            var userFound = await _context.Users.FindAsync(userId);
            if (userFound.Type != TypeUsers.Owner.ToString() && userId != id)
            {
                return RedirectToAction("AccessDenied", "Account");

            }

            var user = await _context.Users.FindAsync(id);
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
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser.Email != user.Email)
                    {
                        // Verifica se o novo email já existe no banco de dados
                        var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
                        if (emailExists)
                        {
                            if (existingUser.Type != "Owner")
                            {
                                TempData["ErrorMessage"] = "Invalid e-mail, try again!";
                                return RedirectToAction("Index", "Properties");
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "This e-mail belongs to another user!";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                    }

                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Email = user.Email;
                    existingUser.Password = user.Password;
                    existingUser.Type = user.Type;
                    existingUser.IsActive = user.IsActive;

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
                var userFound = await _context.Users.FindAsync(userId);
                if (userFound.Type == TypeUsers.Owner.ToString())
                {
                    TempData["SuccessMessage"] = "User updated!";
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
            var userFound = await _context.Users.FindAsync(userId);
            if (userFound.Type != TypeUsers.Owner.ToString() && userId != id)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var user = await _context.Users
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
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                try
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (userId == id)
                    {
                        return RedirectToAction("Logout", "Account");
                    }
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "You can not delete this profile. There are properties related to this account!";
                    return RedirectToAction(nameof(Index));
                }

            }
            TempData["SuccessMessage"] = "User deleted!";
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        public IActionResult Search(string searchTerm)
        {
            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(p => p.FirstName.Contains(searchTerm) || p.LastName.Contains(searchTerm) || p.Email.Contains(searchTerm) || p.Type.Contains(searchTerm));
            }

            return View("Index", users.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                var properties = await _context.Properties.Where(p => p.ManagerId == id).ToListAsync();
                if (properties.Count > 0)
                {
                    return StatusCode(500, "You can not delete your profile, you have properties related with your account!");
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Account deleted!";
                return StatusCode(200);
            }
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
