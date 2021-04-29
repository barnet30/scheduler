

using System.ComponentModel.DataAnnotations;

namespace scheduler.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан никнейм")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
