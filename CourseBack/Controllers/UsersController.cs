using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IActionResult AuthorizeUser([FromBody] UserRequest user)
        {
            var result = _userService.AuthorizeUser(user);
            if (result.Error != null)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.user);
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var result = _userService.GetAllUsers();
            if (result.Error != null)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var result = _userService.GetUser(id);
            if (result.user == null)
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(result.Error))
            {
                return Ok(result.user);
            }

            return BadRequest(result.Error);
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

        [HttpPut]
        public IActionResult UpdateUser([FromBody] User user)
        {
            var result = _userService.UpdateUser(user);
            if (String.IsNullOrEmpty(result))
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var result = _userService.DeleteUser(id);

            if (String.IsNullOrEmpty(result))
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [HttpDelete]
        public IActionResult DeleteAllUsers()
        {
            var result = _userService.DeleteAllUsers();

            if (String.IsNullOrEmpty(result))
            {
                return Ok();
            }
            return BadRequest(result);
        }
    }
}

