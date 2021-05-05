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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventModel model)
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name != null)
            {
                if (ModelState.IsValid)
                {
                    User user = await db.Users.FirstOrDefaultAsync(u => u.Nickname == model.Nickname);
                    if (user != null)
                    {
                        if (model.BeginDate >= DateTime.Now && model.EndDate >= model.BeginDate)
                        {
                            db.Events.Add(new Event { Title = model.Title, CreaterUserId = user.Id, BeginDate = model.BeginDate, EndDate = model.EndDate });
                            await db.SaveChangesAsync();

                            Event newEv = await db.Events.OrderBy(i => i.Id).LastOrDefaultAsync(u => u.CreaterUserId == user.Id);
                            db.DateEvents.Add(new DateEvent { EventId = newEv.Id, UserId = user.Id, BeginDate = model.BeginDate, EndDate = model.EndDate });
                            await db.SaveChangesAsync();

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Неврно заполнена дата проведения");
                            return View();
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "Неверный никнейм");
                        return View();
                    }
                }
                else
                    ModelState.AddModelError("", "Некоректные данные");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public IActionResult Calculate(int id)
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            string createrName = db.Users.FirstOrDefault(u => u.Id == db.Events.FirstOrDefault(e => e.Id == id).CreaterUserId).Nickname;
            if (createrName == HttpContext.User.Identity.Name)
            {
                var bdates = (from dateevent in db.DateEvents
                              where dateevent.EventId == id
                              select dateevent.BeginDate).ToList();

                var edates = (from dateevent in db.DateEvents
                              where dateevent.EventId == id
                              select dateevent.EndDate).ToList();

                DateTime bdate = bdates.Max();
                DateTime edate = edates.Min();

                if (bdate >= edate)
                {
                    ViewBag.Message = "Нет удобного для всех времени";
                    return View();
                }
                else
                {
                    ViewBag.Message = $"Удобное время для проведения события: {bdate.ToString("g")} - {edate.ToString("g")}";
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "Вы не являетесь создателем этого события";
                return View();
            }
            
        }

    }
}
