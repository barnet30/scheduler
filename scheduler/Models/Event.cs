using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace scheduler.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }


        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BeginDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }


        public virtual User User { get; set; }

    }
}
