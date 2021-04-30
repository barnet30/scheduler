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
                    db.Add(new Event { Title = model.Title, UserId = user.Id, EventDate = model.EventDate, BeginTime = model.BeginTime, EndTime = model.EndTime });
                    await db.SaveChangesAsync();
                    
                    return RedirectToAction("Index", "Home");
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
