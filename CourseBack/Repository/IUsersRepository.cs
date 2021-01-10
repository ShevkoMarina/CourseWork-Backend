using CourseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Repository
{
    public interface IUsersRepository
    {
        (IEnumerable<User> Users, string Error) GetAllUsers();

        (User user, string Error) GetUser(int id);

        (string Error, int id) AddUser(UserRequest user);

        string UpdateUser(User user);

        string DeleteUser(int id);

        string DeleteAllUsers();

        (string Error, int id) AuthorizeUser(UserRequest request);
    }
}
