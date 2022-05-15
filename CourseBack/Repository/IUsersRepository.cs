using CourseBack.Models;
using System;

namespace CourseBack.Repository
{
    public interface IUsersRepository
    {
        (string Error, Guid id) AddUser(UserRequest user);

        (string Error, Guid id) AuthorizeUser(UserRequest request);
    }
}