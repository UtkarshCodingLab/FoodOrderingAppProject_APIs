using Microsoft.AspNetCore.Mvc;
using Restaurant_API.Models.Dto;
using Restaurant_API.Repository;
using System.Threading.Tasks;

namespace Restaurant_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            var response = await _authRepository.RegisterAsync(model);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var response = await _authRepository.LoginAsync(model);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
