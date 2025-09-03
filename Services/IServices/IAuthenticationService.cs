using WebApplication.Models.Authentication;

namespace WebApplication.Services.IServices
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginDto);
        Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO registerDto);
        Task<bool> DeleteUserAsync(int userId);
    }
}
