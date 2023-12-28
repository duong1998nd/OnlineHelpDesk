using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineHelpDesk.Models;
using OnlineHelpDesk.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace OnlineHelpDesk.Controllers
{
    [Authorize(Roles = "Admin, Support, User")]
    [Route("login")]
    public class LoginController : Controller
    {
        private AppDbContext dbContext;
        public LoginController(AppDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }
        [AllowAnonymous]
        public IActionResult Admin(string email, string password)
        {
            var acc = checkLogin(email, password);
            return View(acc);

        }
        [Route("index")]
        [Route("")]
        [Route("~/")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("ok")]
        [AllowAnonymous]
        public IActionResult Ok(string email, string password)
        {
            var account = checkLogin(email, password);
            if (account != null)
            {
                AuthenticationManager authentication = new AuthenticationManager();
                authentication.SignIn(HttpContext, account);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.error = "Email does not exist";
                return View("Index");
            }
            
        }

        [Route("SignOut")]
        public IActionResult SignOut()
        {
            AuthenticationManager authentication = new AuthenticationManager();
            authentication.SignOut(HttpContext);
            return RedirectToAction("Index");
        }

        [Route("Denied")]
        public IActionResult Denied()
        {
            ViewBag.loginStaus = "Bạn không có quyền truy cập";
            return View("Denied", "Login");
        }

        [AllowAnonymous]
        private Account checkLogin(string email, string password)
        {
            //var account = dbContext.Account.Select(a=>new AccounDTO()
            //{
            //    Id = a.Id,
            //    FName = a.FName,
            //    Email = a.Email,
            //    Avatar = a.Avatar,
            //    Password = a.Password,
            //    RoleId = a.RoleId,
            //    RoleName = a.Role.Name
            //}).FirstOrDefault(a => a.Email.Equals(email));
            //if(account !=null && account.Password.Equals(password))
            //{
            //    return account;
            //}
            var account = dbContext.Account.Include(a=>a.Role).FirstOrDefault(a => a.Email.Equals(email) && a.Status == true);
            if (account != null)
            { 
                if (account.Password.Equals(password))
                {
                    return account;
                }
            }
            return null;
        }
    }
}