namespace InternetProg2.Models
{
    public class DataResult<T>
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
    }
}
