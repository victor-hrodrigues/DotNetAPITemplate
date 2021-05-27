using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Handlers;
using Template.Models;

namespace Template.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        public UserHandler UserHandler { get; set; }

        public UserController()
        {
            UserHandler = new UserHandler();
        }

        [Route("Login")]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult PostLogin([FromBody] UserInfo userInfo)
        {
            string token = UserHandler.GetUserToken(userInfo);
            if (!string.IsNullOrWhiteSpace(token))
            {
                return Ok(new Tuple<string, UserInfo>(token, userInfo));
            }
            else
            {
                return BadRequest("User unauthorized");
            }
        }
    }
}
