using CourseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseBack.Repository
{
    
    public class UsersRepository : IUsersRepository
    {
        

        private readonly CourseWorkDBContext _context;

        public UsersRepository(CourseWorkDBContext context)
        {
            _context = context;
        }

        public (string Error, Guid id) AddUser(UserRequest user)
        {
            try
            {
                _context.Users.Add(new User { Login = user.Login, Password = user.Password });
                _context.SaveChanges();

                return (null, _context.Users.Last().Id);
            }
            catch (Exception e)
            {
                return (e.Message, Guid.Empty);
            }
        }

        // по идее база должны вернуть только юзера, а уже сервис разбирается в ошибках
        public (string Error, Guid id) AuthorizeUser(UserRequest request)
        {
            try
            {
                var user = _context.Users.ToList()
                    .FirstOrDefault(u => u.Login == request.Login
                    && u.Password == request.Password); 

                if (user != null)
                {
                    return (null, user.Id);
                }

                return ("Wrong password", Guid.Empty);
            }
            catch (Exception e)
            {
                return (e.Message, Guid.Empty);
            }
        }

        public string DeleteAllUsers()
        {
            try
            {
                var adminUser = _context.Users.ToList().Where(u => u.Login == "admin").FirstOrDefault();
                _context.Users.RemoveRange(_context.Users.ToList());
                _context.Users.Add(new User { Login = adminUser.Login, Password = adminUser.Password });
                _context.SaveChanges();
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string DeleteUser(Guid id)
        {
            try
            {
                _context.Users.Remove(_context.Users.Find(id));
                _context.SaveChanges();

                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public (IEnumerable<User> Users, string Error) GetAllUsers()
        {
            try
            {
                return (_context.Users.ToList(), null);
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }
        }

        public (User user, string Error) GetUser(Guid id)
        {
            try
            {
                return (_context.Users.Find(id), null);
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }
        }

        public string UpdateUser(User user)
        {
            try
            {
                var userOld = _context.Users.Find(user.Id);

                userOld.Login = user.Login;
                userOld.Password = user.Password;

                _context.SaveChanges();

                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
    
}
