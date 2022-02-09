namespace JamOrder.API.Dtos
{
    public class ApiReponse<T> where T: class
    {
        public T data { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }

        public ApiReponse(T data, string message, int statusCode)
        {
            this.data = data;
            this.message = message;
            this.statusCode = statusCode;
        }
    }
}