using System;
namespace JamOrder.API.Dtos
{
    public class LoginResponseDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccessToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}
