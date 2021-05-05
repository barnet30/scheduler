using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using scheduler.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace scheduler.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        private static List<Event> events;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationContext db)
        {
            _logger = logger;
            this.db = db;
            events = db.Events.ToList();
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            return View(events);
        }

        
        public IActionResult Privacy()
        {
            ViewData["Username"] = HttpContext.User.Identity.Name;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
