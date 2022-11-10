using Microsoft.AspNetCore.Identity;

namespace Assignment1.Models
{
    public class User : IdentityUser
    {
        public string Address { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class IdentifyUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class LoginUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
