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
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime BeginTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }

    }
}
