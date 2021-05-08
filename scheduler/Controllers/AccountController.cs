using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scheduler.Models;
using scheduler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace scheduler.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext db;
        public AccountController(ApplicationContext context)
        {
            this.db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                return View(model);
            }

            User user = await db.Users.FirstOrDefaultAsync(u => u.Nickname == model.Nickname && u.Password == model.Password);
            if (user != null)
            {
                await Authenticate(model.Nickname); // аутентификация
                return RedirectToAction("Create", "Event");
            }
            else
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = await db.Users.FirstOrDefaultAsync(u => u.Nickname == model.Nickname);
            if (user == null)
            {
                // добавляем пользователя в бд
                db.Users.Add(new User { Nickname = model.Nickname, Password = model.Password });
                await db.SaveChangesAsync();
                await Authenticate(model.Nickname); // аутентификация
                return RedirectToAction("Create", "Event");
            }
            else
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
