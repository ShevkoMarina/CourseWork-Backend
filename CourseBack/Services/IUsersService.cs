using CourseBack.Models;
using System;
using System.Collections.Generic;

namespace CourseBack.Services
{
    public interface IUsersService
    {
        (IEnumerable<User> Users, string Error) GetAllUsers();

        (User user, string Error) GetUser(Guid id);

        (string Error, Guid id) AddUser(UserRequest user);

        string UpdateUser(User user);

        string DeleteUser(Guid id);

        string DeleteAllUsers();

        (string Error, Guid id) AuthorizeUser(UserRequest request);
    }
}
