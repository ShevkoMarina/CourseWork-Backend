using CourseBack.Models;
using System;
using System.Collections.Generic;
using CourseBack.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Services
{
    public class UsersService : IUsersService
    {
        private IUsersRepository _userRepository;
        public UsersService(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public (string Error, int id) AddUser(UserRequest user)
        {
            return _userRepository.AddUser(user);
        }

        public (string Error, int id) AuthorizeUser(UserRequest request)
        {
            return _userRepository.AuthorizeUser(request);
        }

        public string DeleteAllUsers()
        {
            return _userRepository.DeleteAllUsers();
        }

        public string DeleteUser(int id)
        {
            return _userRepository.DeleteUser(id);
        }

        public (IEnumerable<User> Users, string Error) GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public (User user, string Error) GetUser(int id)
        {
            return _userRepository.GetUser(id);
        }

        public string UpdateUser(User user)
        {
            return _userRepository.UpdateUser(user);
        }
    }
}
