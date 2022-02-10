using System.ComponentModel.DataAnnotations;

namespace JamOrder.API.Dtos
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
