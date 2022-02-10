using JamOrder.API.Dtos;

namespace JamOrder.API.Service
{
    public interface ITokenService
    {
        bool DestroyToken(string userName, string encryptedToken);
        TokenResponseDto GenerateToken(string userName);
    }
}
