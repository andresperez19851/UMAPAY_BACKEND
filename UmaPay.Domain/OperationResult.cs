namespace UmaPay.Domain
{
    public class OperationResult<T>
    {
        public bool Success { get; private set; }
        public T Data { get; private set; }
        public string Message { get; private set; }

        private OperationResult(bool isSuccess, T data, string message)
        {
            Success = isSuccess;
            Data = data;
            Message = message;
        }

        public static OperationResult<T> SuccessResult(T data) =>
            new OperationResult<T>(true, data, null);

        public static OperationResult<T> Failure(string message) =>
            new OperationResult<T>(false, default, message);

        public static OperationResult<T> Failure(string message, T data) =>
            new OperationResult<T>(false, data, message);

        public static OperationResult<T> SuccessResult(string message) =>
            new OperationResult<T>(true, default, message);

        public static implicit operator OperationResult<T>(string message)
            => Failure(message);


        public static implicit operator OperationResult<T>(T data)
        {
            if (data != null)
            {
                return SuccessResult(data);
            }

            throw new ArgumentNullException(nameof(data), "Data is null");
        }
    }
}

