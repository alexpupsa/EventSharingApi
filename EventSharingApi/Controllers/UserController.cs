using EventSharingApi.Models;
using EventSharingApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EventSharingApi.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            if (user.Username == null)
            {
                return BadRequest();
            }

            var result = await _userRepository.Register(user.Username);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
