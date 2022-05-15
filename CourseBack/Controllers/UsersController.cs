using System;
using CourseBack.Models;
using CourseBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseBack.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUsersService _userService;

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult AddUser([FromBody] UserRequest user)
        {
            var result = _userService.AddUser(user);
            if (String.IsNullOrEmpty(result.Error))
            {
                return Ok(result.id);
            }
            return BadRequest(result.Error);
        }
    }
}