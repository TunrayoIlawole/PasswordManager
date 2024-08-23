namespace PasswordManager.Responses {
    public class ResponseData<T> : ServiceResponse {
        public T Data { get; set; }

        public ResponseData() {}
    }
}