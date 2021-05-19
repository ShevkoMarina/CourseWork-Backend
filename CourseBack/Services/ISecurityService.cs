using CourseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Services
{
    public interface ISecurityService
    {
        (string, AuthorizeUserResponse) AuthorizeUser(UserRequest request);
    }
}
