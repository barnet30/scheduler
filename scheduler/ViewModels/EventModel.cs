using System;
using System.ComponentModel.DataAnnotations;

namespace scheduler.ViewModels
{
    public class EventModel
    {
        [StringLength(60, MinimumLength = 3)]
        [Required(ErrorMessage = "Неверно указано название")]
        public string Title{ get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Nickname { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BeginDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

    }
}
