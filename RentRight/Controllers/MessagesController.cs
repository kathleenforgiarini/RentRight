using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol.Plugins;
using RentRight.Data;
using RentRight.Models;
using RentRight.Models.Enums;

namespace RentRight.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly RentRightContext _context;

        public MessagesController(RentRightContext context)
        {
            _context = context;
        }

        // GET: Messages
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

            var userAuthId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var rentRightContext = _context.Message
                                .Include(m => m.Apartment)
                                .Include(m => m.Sender)
                                .Include(m => m.Receiver)
                                .Where(u => u.SenderId == userAuthId || u.ReceiverId == userAuthId)
                                .OrderBy(u=>u.SendDate);

            var messages = await rentRightContext.ToListAsync();

            var groupedMessages = messages.GroupBy(m => m.Topic)
                                  .Select(group => group.First());

            return View(groupedMessages.GroupBy(m => m.Topic));
        }

        [HttpPost]
        public async Task<IActionResult> SendFirstMessage(int apartmentId, int receiverId)
        {
            var apartment = await _context.Apartment.Include(p=>p.Property).FirstOrDefaultAsync(a => a.Id == apartmentId);
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == receiverId);

            ViewBag.Apartment = apartment;
            ViewBag.User = user;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage([Bind("SenderId,ReceiverId,Topic,Content,ApartmentId")] Models.Message message)
        {
            if (ModelState.IsValid)
            {
                message.SendDate = DateTime.Now;
                _context.Add(message);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Message sent!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ViewMessage(string messageTopic)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;

            var userAuthId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var message = await _context.Message.Include(p => p.Sender).Include(p => p.Receiver).Include(p => p.Apartment).Include(p => p.Apartment.Property)
                .FirstOrDefaultAsync(p => p.Topic == messageTopic);

            if (message == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (message.ReceiverId != userAuthId && message.SenderId != userAuthId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            List<Models.Message> messages = await _context.Message.Where(m=> m.Topic == message.Topic).OrderBy(m=>m.SendDate).ToListAsync();
            ViewBag.Messages = messages;
            ViewBag.Apartment = message.Apartment;
            ViewBag.Topic = message.Topic;
            ViewBag.Receiver = message.Receiver;
            ViewBag.Sender = message.Sender;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendNewMessage(int senderId, int receiverId, string content, string topic, int apartmentId)
        {
            Models.Message message = new Models.Message();
            message.SenderId = senderId;
            message.ReceiverId = receiverId;
            message.ApartmentId = apartmentId;
            message.Content = content;
            message.Topic = topic;
            message.SendDate = DateTime.Now;

            try
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Message sent!";
                return RedirectToAction("ViewMessage", new { messageTopic = message.Topic});
            }
            catch (Exception ex)
            {                    
                TempData["ErrorMessage"] = "Message could not be sent. Try again!";
                return RedirectToAction("Index");
            }
        }
    }
}
