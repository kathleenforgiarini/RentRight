using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentRight.Data;
using RentRight.Models;
using RentRight.Models.Enums;

namespace RentRight.Controllers
{
    [Authorize(Policy = "RequireTenantOrManagerRole")]
    public class AppointmentsController : Controller
    {
        private readonly RentRightContext _context;

        public AppointmentsController(RentRightContext context)
        {
            _context = context;
        }

        // GET: Appointments
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var rentRightContext = _context.Appointments.Include(a => a.Apartment).Include(a => a.Apartment.Property).Include(a => a.Manager).Include(a => a.Tenant);
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            return View(await rentRightContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment (int apartmentId, int managerId)
        {
            var apartment = await _context.Apartment
                                .Include(a => a.Property)
                                .FirstOrDefaultAsync(a => a.Id == apartmentId);
            ViewBag.Apartment = apartment;
            ViewBag.ManagerId = managerId;
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int apartmentId, int managerId, string selectedTime, string date)
        {
            Appointments appointments = new Appointments();
            appointments.ApartmentId = apartmentId;
            appointments.ManagerId = managerId;
            String dateTimeString = date + " " + selectedTime;
            DateTime dateTime = DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            appointments.AppointmentDate = dateTime;
            var tenantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            appointments.TenantId = tenantId;

            if (ModelState.IsValid)
            {
                _context.Add(appointments);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Visit scheduled!";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("CreateAppointment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments
                                .FirstOrDefaultAsync(a => a.Id == appointmentId);
            appointment.Status = AppointmentStatus.Confirmed.ToString();
            await _context.SaveChangesAsync();

            var dayOfWeek = appointment.AppointmentDate.DayOfWeek.ToString();
            var time = appointment.AppointmentDate.TimeOfDay;

            var managerAvailabilities = _context.ManagerAvailability
                .Where(av => av.ManagerId == appointment.ManagerId && av.DayOfTheWeek == dayOfWeek && !av.IsScheduled)
                .ToList();

            var managerAvailability = managerAvailabilities.FirstOrDefault(av => av.Time == time);

            if (managerAvailability != null)
            {
                managerAvailability.IsScheduled = true;
                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Appointment confirmed!";
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments
                                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            var dayOfWeek = appointment.AppointmentDate.DayOfWeek.ToString();
            var time = appointment.AppointmentDate.TimeOfDay;

            var managerAvailabilities = _context.ManagerAvailability
                .Where(av => av.ManagerId == appointment.ManagerId && av.DayOfTheWeek == dayOfWeek)
                .ToList();

            var managerAvailability = managerAvailabilities.FirstOrDefault(av => av.Time == time);

            if (managerAvailability != null)
            {
                managerAvailability.IsScheduled = false;
                await _context.SaveChangesAsync();
            }

            if (appointment.Status == AppointmentStatus.Canceled.ToString())
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Appointment deleted!";
                return RedirectToAction(nameof(Index));

            }

            TempData["SuccessMessage"] = "Appointment canceled!";
            appointment.Status = AppointmentStatus.Canceled.ToString();
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult GetAvailableTimes(string dayOfWeek)
        {
            var availableTimes = _context.ManagerAvailability
                .Where(av => av.DayOfTheWeek == dayOfWeek && av.IsScheduled == false)
                .Select(av => av.Time)
                .ToList();

            return Json(availableTimes);
        }
    }
}
