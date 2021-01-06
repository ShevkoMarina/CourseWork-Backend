using CourseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly CourseWorkDatabaseContext _context;

        public UsersRepository(CourseWorkDatabaseContext context)
        {
            _context = context;
        }

        public (string Error, int id) AddUser(UserRequest user)
        {
            try
            {
                _context.Users.Add(new User { Login = user.Login, Password = user.Password });
                _context.SaveChanges();

                return (null, _context.Users.ToList().Last().Id);
            }
            catch (Exception e)
            {
                return (e.Message, -1);
            }
        }

        public (string Error, User user) AuthorizeUser(UserRequest request)
        {
            try
            {
                var user = _context.Users.ToList()
                    .FirstOrDefault(u => u.Login == request.Login
                    && u.Password == request.Password); 

                if (user != null)
                {
                    return (null, user);
                }

                return ("Wrong password", null);
            }
            catch (Exception e)
            {
                return (e.Message, null);
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

        public string DeleteUser(int id)
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

        public (User user, string Error) GetUser(int id)
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
