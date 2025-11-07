namespace UmaPay.Domain
{

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Path { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
