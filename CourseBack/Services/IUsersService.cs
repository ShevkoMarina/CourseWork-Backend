using CourseBack.Models;
using System;

namespace CourseBack.Services
{
    public interface IUsersService
    {
        (string Error, Guid id) AddUser(UserRequest user);
    }
}