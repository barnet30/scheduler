using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scheduler.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }

        public List<Event> Events { get; set; }
        public List<DateEvent> DateEvents { get; set; }
    }
}
