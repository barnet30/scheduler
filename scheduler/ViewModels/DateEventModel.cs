using scheduler.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace scheduler.ViewModels
{
    public class DateEventModel
    {
        /*[StringLength(60, MinimumLength = 3)]
        [Required(ErrorMessage = "Неверно указано название")]
        public string Title { get; set; }*/

        public IEnumerable<Event> Events { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required(ErrorMessage = "Неверно указан NICKNAME")]
        public string Nickname { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BeginDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

    }
}
