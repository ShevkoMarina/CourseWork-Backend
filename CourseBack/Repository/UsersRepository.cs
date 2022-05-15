using CourseBack.Models;
using System;
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

                return (null, _context.Users.Where(x => x.Login == user.Login).FirstOrDefault().Id);
            }
            catch (Exception e)
            {
                return (e.Message, Guid.Empty);
            }
        }

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
    }
}