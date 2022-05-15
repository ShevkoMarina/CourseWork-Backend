using CourseBack.Models;
using System;
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
    }
}
