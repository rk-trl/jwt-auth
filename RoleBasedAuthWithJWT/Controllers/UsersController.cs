using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleBasedAuthWithJWT.Entities;
using RoleBasedAuthWithJWT.Services;

namespace RoleBasedAuthWithJWT.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]User userParam)
        {
            var user = _userService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult Get()
        {
            var users =  _userService.GetAll();
            return Ok(users);
        }

       [Authorize(Roles = "User")]
        [HttpGet("GetUserDetails")]
        public IActionResult GetUserDetails()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}
