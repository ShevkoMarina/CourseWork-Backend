using CourseBack.Models;
using System;
using System.Collections.Generic;
using CourseBack.Repository;

namespace CourseBack.Services
{
    public class UsersService : IUsersService
    {
        private IUsersRepository _userRepository;
        public UsersService(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public (string Error, Guid id) AddUser(UserRequest user)
        {
            return _userRepository.AddUser(user);
        }

        public (string Error, Guid id) AuthorizeUser(UserRequest request)
        {
            return _userRepository.AuthorizeUser(request);
        }

        public string DeleteAllUsers()
        {
            return _userRepository.DeleteAllUsers();
        }

        public string DeleteUser(Guid id)
        {
            return _userRepository.DeleteUser(id);
        }

        public (IEnumerable<User> Users, string Error) GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public (User user, string Error) GetUser(Guid id)
        {
            return _userRepository.GetUser(id);
        }

        public string UpdateUser(User user)
        {
            return _userRepository.UpdateUser(user);
        }
    }
}
