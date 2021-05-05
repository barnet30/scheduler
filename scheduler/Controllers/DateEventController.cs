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
        public IActionResult Subscribe(int id)
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            Event tmpEv = db.Events.FirstOrDefault(el => el.Id == id);
            if (tmpEv != null) 
            {
                DateEventModel model = new DateEventModel
                {
                    Events = db.Events,
                    SelectedEvent = tmpEv.Title,
                    BeginDate = tmpEv.BeginDate,
                    EndDate = tmpEv.EndDate
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(DateEventModel model, string SelectedEvent)
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            DateEventModel errModel = new DateEventModel
            {
                Events = db.Events,
                SelectedEvent = SelectedEvent,
                BeginDate = model.BeginDate,
                EndDate = model.EndDate
            };

            if (ModelState.IsValid)
            {
                User user = db.Users.FirstOrDefault(u => u.Nickname == model.Nickname);
                if (user != null)
                {
                    //bool check = (allUsersId.Contains(user.Id));
                    Event ev = db.Events.FirstOrDefault(u => u.Title == SelectedEvent);
                    if (ev != null) 
                    {
                        var userId = db.DateEvents.FirstOrDefault(u => u.UserId == user.Id && u.EventId == ev.Id);
                        if (userId == null)
                        {
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
                        else
                        {
                            ModelState.AddModelError("", "Такой пользователь уже записался на событие");
                            return View(errModel);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Такого события не существует");
                        return View(errModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Такого пользователя не существует!");
                    return View(errModel);
                }
            }
            else
                ModelState.AddModelError("", "Некоректные данные");
            return View(errModel);
        }
    }
}
