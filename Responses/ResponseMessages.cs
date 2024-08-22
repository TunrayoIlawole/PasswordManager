namespace PasswordManager.Responses {
    public static class ResponseMessages {
        public static string Success => "sucess";
        public static string Failure => "error";
        public static string LoginSuccess = "User logged in successfully";

        public static string InvalidPassword => "Password not found";

        public static string InvalidPasswords => "Passwords not found";
         public static string InvalidClaim(string claimName) => $"The {claimName} is missing";

        public static string DuplicateUser(string email) => $"User with email {email} already exists";
        
    }
}