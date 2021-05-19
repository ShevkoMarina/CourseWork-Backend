using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Models
{
    public class AuthorizeUserResponse
    {
        public string AccessToken { get; set; }

        public string UserId { get; set; }
    }
}
