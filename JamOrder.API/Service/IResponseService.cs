using JamOrder.API.Dtos;

namespace JamOrder.API.Service
{
    public interface IResponseService
    {
        ApiReponse<T> Response<T>(T data, string message, int statusCode) where T : class;
    }
}
