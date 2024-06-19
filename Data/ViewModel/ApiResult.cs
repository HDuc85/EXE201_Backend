namespace Data.ViewModel
{
    public class ApiResult<T>
    {
        public T? Value { get; set; }
        public bool Success { get; set; }
 
        public string message { get; set; }
    }
    public class ApiResult
    {
        public bool Success { get; set; }

        public string message { get; set; }
    }
}
