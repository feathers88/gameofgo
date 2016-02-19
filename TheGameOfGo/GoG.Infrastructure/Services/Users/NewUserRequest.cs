namespace GoG.Infrastructure.Services.Users
{
    public class NewUserRequest
    {
        public NewUserRequest()
        {
            // Need the empty constructor so serializer can create the object.  But we don't use it directly.
        }

        public NewUserRequest(string userId, string password, string email)
        {
            UserId = userId;
            Password = password;
            Email = email;
            //FirstName = firstName;
            //LastName = lastName;
        }

        public string UserId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
    }
}
