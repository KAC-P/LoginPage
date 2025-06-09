using System.Collections.Generic;

namespace SystemLogowaniaAdmina.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<string> AccessTo { get; set; }
    }
}
