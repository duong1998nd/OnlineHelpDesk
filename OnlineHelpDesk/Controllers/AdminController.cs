using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineHelpDesk.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineHelpDesk.Controllers
{
    [Route("account")]

    public class AdminController : Controller
    {
        private AppDbContext dbContext;

        public AdminController(AppDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("list")]
        public IActionResult ListAccount()
        {
            ViewBag.users = dbContext.Account.Include(a=>a.Role)
                .Where(a=> a.RoleId != 1)
                .ToList();
            return View("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("category")]
        public IActionResult ListCategory()
        {
            ViewBag.cate = dbContext.Category.ToList();
            Console.WriteLine(TempData["Message"]+" sdfdsdsf");
            TempData["test"] = "Data test";
            return View("Category");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("period")]
        public IActionResult ListPeriod()
        {
            ViewBag.period = dbContext.Period.ToList();
            return View("Period");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("status")]
        public IActionResult ListStatus()
        {
            ViewBag.status = dbContext.Status.ToList();
            return View("Status");
        }

        [Authorize(Roles = "Admin, Support")]
        [HttpGet]
        [Route("details")]
        public IActionResult Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var user = dbContext.Account.FirstOrDefault(u => u.Email.Equals(userEmail));
            return View("Indexx", user);
        }

        [Authorize(Roles = "Admin, Support")]
        [HttpPost]
        [Route("details")]
        public IActionResult Index(Account acc)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var userr = dbContext.Account.FirstOrDefault(u => u.Email.Equals(userEmail));

            try
            {
                userr.FName = acc.FName;
                userr.Password = acc.Password;
                userr.Email = acc.Email;
                userr.Status = acc.Status;
                dbContext.SaveChanges();
                ViewBag.msg = "Oke";
            }
            catch (Exception e)
            {
                ViewBag.msg = "Failed";
            }
            return View("Indexx", userr);
        }
    }
}