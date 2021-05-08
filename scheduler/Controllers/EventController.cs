using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scheduler.Models;
using scheduler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scheduler.Controllers
{
    public class EventController : Controller
    {
        private ApplicationContext db;
        public EventController(ApplicationContext context)
        {
            this.db = context;
        }
         
        
        [HttpGet]
        public IActionResult Info(int id)
        {
            Event model = db.Events.FirstOrDefault(el => el.Id == id);
            ViewData["Nick"] = db.Users.FirstOrDefault(c => c.Id == model.CreaterUserId).Nickname;
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventModel model)
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;

            // Если пользователь не авторизован
            if (HttpContext.User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Некоректные данные");
                return View();
            }

            User user = await db.Users.FirstOrDefaultAsync(u => u.Nickname == model.Nickname);
            if (user == null)
            {
                ModelState.AddModelError("", "Неверный никнейм");
                return View();
            }

            if (model.BeginDate >= DateTime.Now && model.EndDate >= model.BeginDate)
            {
                db.Events.Add(new Event { Title = model.Title, CreaterUserId = user.Id, BeginDate = model.BeginDate, EndDate = model.EndDate });
                await db.SaveChangesAsync();

                Event newEvent = await db.Events.OrderBy(i => i.Id).LastOrDefaultAsync(u => u.CreaterUserId == user.Id);
                db.DateEvents.Add(new DateEvent { EventId = newEvent.Id, UserId = user.Id, BeginDate = model.BeginDate, EndDate = model.EndDate });
                await db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Неврно заполнена дата проведения");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CalculateDates (int id)
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            User creater = await db.Users.FirstOrDefaultAsync(u => u.Id == db.Events.FirstOrDefault(e => e.Id == id).CreaterUserId);
            string createrName = creater.Nickname;
            if (createrName != HttpContext.User.Identity.Name)
            {
                ViewBag.Message = "Вы не являетесь создателем этого события";
                return View();
            }

            var beginDates = db.DateEvents.Where(e => e.EventId == id).Select(e => e.BeginDate).ToList();

            var endDates = db.DateEvents.Where(e => e.EventId == id).Select(e => e.EndDate).ToList();

            DateTime beginDate = beginDates.Max();
            DateTime endDate = endDates.Min();

            if (beginDate >= endDate)
            {
                ViewBag.Message = "Нет удобного для всех времени";
                return View();
            }
            else
            {
                ViewBag.Message = $"Удобное время для проведения события: {beginDate.ToString("g")} - {endDate.ToString("g")}";
                return View();
            }            
        }

    }
}
