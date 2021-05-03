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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventModel model)
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

                        Event newEv = await db.Events.OrderBy(i=>i.Id).LastOrDefaultAsync(u => u.CreaterUserId == user.Id);
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

    }
}
