using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineHelpDesk.Helpers;
using OnlineHelpDesk.Hubs;
using OnlineHelpDesk.Models;

namespace OnlineHelpDesk.Controllers
{
    public class TicketsController : Controller
    {
        private readonly AppDbContext _context;
        public TicketsController(AppDbContext context, IHubContext<ChatHub> signlrHub)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Ticket.Include(t => t.Category).Include(t => t.Period).Include(t => t.Status).Include(t => t.Supporter).Include(t => t.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        [Authorize(Roles ="Admin, User, Supporter")]
        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int? id)
        {
            ViewBag.ticket = _context.Ticket
                .Include(t => t.Category)
                .Include(t => t.Photo)
                .Include(t => t.Period)
                .Include(t => t.Status)
                .Include(t => t.Supporter)
                .Include(t => t.User)
                .FirstOrDefault(m => m.Id == id);
            ViewBag.supporters = _context.Account
                .Include(a=>a.Role)
                .Where(a => a.RoleId == 3 && a.Status==true).ToList();

            ViewBag.discusion = _context.Discussion
                .Include(t => t.Account)
                .Include(t => t.Ticket)
                .Where(t => t.TickerId == id)
                .OrderByDescending(t=>t.Id)
                .ToList()
                .Take(6);

            return View("Details");
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["PeriodId"] = new SelectList(_context.Period, "Id", "Id");
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Account, "Id", "Email");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tittle,Description,CreatedDate,CategoryId,PeriodId,SupporterId,UserId,StatusId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userEmail = User.FindFirst(ClaimTypes.Email).Value;
                    var currentUser = _context.Account.FirstOrDefault(a => a.Email.Equals(userEmail));
                    ticket.CreatedDate = DateTime.Now;
                    ticket.UserId = currentUser.Id;
                    _context.Add(ticket);

                    await _context.SaveChangesAsync();

                    var files = HttpContext.Request.Form.Files;
                    if (files.Count() > 0 && files[0].Length > 0)
                    {
                        var photo = new Photo();
                        var file = files[0];
                        var fileName = file.FileName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            photo.Name = fileName;
                            photo.TicketId = ticket.Id;
                            _context.Photo.Add(photo);
                            _context.SaveChanges();
                        }
                    }
                    TempData["msg"] = "Done";
                }
                catch
                {
                    TempData["msg"] = "Failed";
                }
                
                return RedirectToAction("history", "tickets");
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", ticket.CategoryId);
            ViewData["PeriodId"] = new SelectList(_context.Period, "Id", "Id", ticket.PeriodId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", ticket.StatusId);
            ViewData["UserId"] = new SelectList(_context.Account, "Id", "Email", ticket.UserId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", ticket.CategoryId);
            ViewData["PeriodId"] = new SelectList(_context.Period, "Id", "Id", ticket.PeriodId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", ticket.StatusId);
            ViewData["SupporterId"] = new SelectList(_context.Account, "Id", "Email", ticket.SupporterId);
            ViewData["UserId"] = new SelectList(_context.Account, "Id", "Email", ticket.UserId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tittle,Description,CreatedDate,CategoryId,PeriodId,SupporterId,UserId,StatusId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", ticket.CategoryId);
            ViewData["PeriodId"] = new SelectList(_context.Period, "Id", "Id", ticket.PeriodId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", ticket.StatusId);
            ViewData["SupporterId"] = new SelectList(_context.Account, "Id", "Email", ticket.SupporterId);
            ViewData["UserId"] = new SelectList(_context.Account, "Id", "Email", ticket.UserId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.Category)
                .Include(t => t.Period)
                .Include(t => t.Status)
                .Include(t => t.Supporter)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            _context.Ticket.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles ="User")]
        [HttpGet]
        public IActionResult History()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var acc = _context.Account.FirstOrDefault(a => a.Email.Equals(userEmail));
            ViewBag.tickets = _context.Ticket.Include(t=>t.Status).Include(t=>t.Period).Include(t=>t.Category).Where(t => t.UserId == acc.Id).ToList();
            return View("History");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult List()
        {
            ViewBag.tickets = _context.Ticket
                .Include(t => t.Status)
                .Include(t=>t.User)
                .Include(t=>t.Supporter)
                .Include(t => t.Period)
                .Include(t => t.Category)
                .ToList();
            return View("List");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Assign()
        {
            ViewBag.tickets = _context.Ticket.Include(t => t.Status)
                .Include(t => t.User)
                .Include(t=>t.Supporter)
                .Include(t => t.Period)
                .Include(t => t.Category)
                .Where(t=>t.Supporter == null)
                .ToList();
            return View("Assign");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Assign(int id, int supporterId)
        {
            var ticket = _context.Ticket
                .Include(t=>t.Supporter)
                .Include(t=>t.User)
                .FirstOrDefault(t=>t.Id == id);
            ticket.SupporterId = supporterId;
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        [Authorize(Roles = "Supporter")]
        [HttpGet]
        public IActionResult Assigned()
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var spt = _context.Account.FirstOrDefault(sp => sp.Email.Equals(email));

            ViewBag.tickets = _context.Ticket.Include(t => t.Status)
                .Include(t => t.User)
                .Include(t => t.Supporter)
                .Include(t => t.Period)
                .Include(t => t.Category)
                .Where(t => t.SupporterId == spt.Id)
                .ToList();
            return View("Assigned");
        }

        [Authorize(Roles = "User, Supporter")]
        [HttpPost]
        [Route("send")]
        public IActionResult Send(int ticketId, string message)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var acc = _context.Account.FirstOrDefault(sp => sp.Email.Equals(email));

            var disc = new Discussion();
            disc.CreateDate = DateTime.Now;
            disc.Content = message;
            disc.AccountId = acc.Id;
            disc.TickerId = ticketId;
            _context.Add(disc);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = ticketId});
        }
        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }
    }
}
