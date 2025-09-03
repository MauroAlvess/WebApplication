using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication.Models.Authentication;
using WebApplication.Services.IServices;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDTO>> Register([FromBody] RegisterRequestDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
        }

        [HttpDelete("user")]
        [Authorize]
        public async Task<ActionResult> DeleteUser()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(new { Success = false, Message = "Usuário não identificado" });
            }

            var result = await _authService.DeleteUserAsync(userId);

            if (!result)
            {
                return NotFound(new { Success = false, Message = "Usuário não encontrado ou já foi excluído" });
            }

            return Ok(new { Success = true, Message = "Usuário excluído com sucesso" });
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult GetCurrentUser()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            var nameClaim = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest(new { Success = false, Message = "Token inválido" });
            }

            return Ok(new
            {
                Success = true,
                UserId = int.Parse(userIdClaim),
                Email = emailClaim,
                Name = nameClaim
            });
        }
    }
}
