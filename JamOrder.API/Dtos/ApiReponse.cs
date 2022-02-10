namespace JamOrder.API.Dtos
{
    public class ApiReponse<T> where T: class
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ApiReponse(T data, string message, int statusCode)
        {
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }
    }
}