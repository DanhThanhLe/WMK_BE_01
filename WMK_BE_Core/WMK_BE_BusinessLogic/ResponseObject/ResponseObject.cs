namespace WMK_BE_BusinessLogic.ResponseObject
{
    public class ResponseObject<T>
    {
        public int StatusCode { get; set; } = 500;
        public string Message { get; set; } = "The server has encountered a situation it does not know how to handle";
        public T? Data { get; set; }

    }
}