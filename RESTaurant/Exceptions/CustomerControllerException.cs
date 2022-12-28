namespace RESTaurant.Exceptions {
    public class CustomerControllerException : Exception {
        public CustomerControllerException(string? message) : base(message) {
        }

        public CustomerControllerException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
