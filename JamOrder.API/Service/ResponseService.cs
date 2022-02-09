using JamOrder.API.Dtos;

namespace JamOrder.API.Service
{
    public class ResponseService : IResponseService
    {
        public ApiReponse<T> Response<T>(T data, string message, int statusCode ) where T : class
        {
            return new ApiReponse<T>(data, message, statusCode);
        }
    }
}
