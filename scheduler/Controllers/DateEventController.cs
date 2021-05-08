using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scheduler.Models;
using scheduler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scheduler.Controllers
{
    public class DateEventController : Controller
    {
        private ApplicationContext db;
        public DateEventController(ApplicationContext context)
        {
            db = context;
            //allUsersId = db.DateEvents.Select(u => u.UserId).Where().ToList() ;
        }

        [HttpGet]
        public async Task<IActionResult> Subscribe(int id)
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Event curr_event = await db.Events.FirstOrDefaultAsync(el => el.Id == id);
            if (curr_event == null)
            {
                return RedirectToAction("Error", "Home");
            }

            DateEventModel model = new DateEventModel
            {
                Events = db.Events,
                SelectedEvent = curr_event.Title,
                BeginDate = curr_event.BeginDate,
                EndDate = curr_event.EndDate
            };
            return View(model);                      
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(DateEventModel model, string SelectedEvent)
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }

            DateEventModel errModel = new DateEventModel
            {
                Events = db.Events,
                SelectedEvent = SelectedEvent,
                BeginDate = model.BeginDate,
                EndDate = model.EndDate
            };

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Некоректные данные");
                return View(errModel);
            }

            User user = await db.Users.FirstOrDefaultAsync(u => u.Nickname == model.Nickname);
            if (user == null)
            {
                ModelState.AddModelError("", "Такого пользователя не существует!");
                return View(errModel);
            }

            Event ev =  await db.Events.FirstOrDefaultAsync(u => u.Title == SelectedEvent);
            if (ev == null)
            {
                ModelState.AddModelError("", "Такого события не существует");
                return View(errModel);
            }


            if (model.BeginDate >= DateTime.Now && model.EndDate >= model.BeginDate)
            {
                db.DateEvents.Add(new DateEvent { EventId = ev.Id, UserId = user.Id, BeginDate = model.BeginDate, EndDate = model.EndDate });
                await db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Неврно заполнена дата проведения");
                return View(errModel);
            }
        }
    }
}
