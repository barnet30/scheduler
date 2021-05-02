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
        private static List<int> allUsersId;
        public DateEventController(ApplicationContext context)
        {
            db = context;
            allUsersId = db.DateEvents.Select(u => u.UserId).ToList() ;
        }


        public IEnumerable<Event> AllEvents => db.Events;

        [HttpGet]
        public IActionResult Subscribe(int id)
        {
            Event tmpEv = db.Events.FirstOrDefault(el => el.Id == id);
            //ViewData["EventName"] = tmpEv.Title;
            //DateEvent de = db.DateEvents.Include(u => u.Event).FirstOrDefault(de => de.Event.Id == id);
            //ViewBag.Name = tmpEv.Title;

            DateEventModel model = new DateEventModel
            {
                Events = AllEvents
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(DateEventModel model, int id)
        {


            //ViewBag.id = id;
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Nickname == model.Nickname);
                //Event ev = await db.Events.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    bool check = (allUsersId.Contains(user.Id));
                    if (!check)
                    {
                        if (model.BeginDate >= DateTime.Now && model.EndDate >= model.BeginDate)
                        {
                            db.DateEvents.Add(new DateEvent { EventId = id, UserId = user.Id, BeginDate = model.BeginDate, EndDate = model.EndDate });
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
                        ModelState.AddModelError("", "Такой пользователь уже записался на событие");
                        return View();
                    }


                }
                else
                {
                    ModelState.AddModelError("", "Такого пользователя не существует!");
                    return View(model);
                }
            }
            else
                ModelState.AddModelError("", "Некоректные данные");
                
            return View(model);
        }
    }
}
