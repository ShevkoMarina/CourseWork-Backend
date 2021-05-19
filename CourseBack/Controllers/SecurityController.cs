using CourseBack.Models;
using CourseBack.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CourseBack.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SecurityController : Controller
    {
        private ISecurityService _accountService;

        public SecurityController(ISecurityService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public IActionResult AuthUser([FromBody] UserRequest request)
        {
            (string error, AuthorizeUserResponse response) = _accountService.AuthorizeUser(request);

            if (!String.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            return Ok(response);
        }
    }
}
