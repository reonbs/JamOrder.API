using JamOrder.API.Dtos;
using System.Threading.Tasks;

namespace JamOrder.API
{
    public interface IUserService
    {
        Task<ApiReponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginDto);
        ApiReponse<object> LogOut();
        Task<ApiReponse<object>> Register(RegisterRequestDto registerRequestDto);
    }
}
