using System.ComponentModel.DataAnnotations;

namespace JamOrder.API.Dtos
{
    public class LogoutRequestDto
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
