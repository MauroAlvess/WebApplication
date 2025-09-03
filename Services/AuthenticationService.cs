using BCrypt.Net;
using WebApplication.Data.Entities;
using WebApplication.Models.Authentication;
using WebApplication.Repositories.IRepositories;
using WebApplication.Services.IServices;

namespace WebApplication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authRepository;
        private readonly JwtService _jwtService;

        public AuthenticationService(IAuthenticationRepository authRepository, JwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginDto)
        {
            try
            {
                // Buscar usuário por email
                var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Email ou senha inválidos"
                    };
                }

                // Verificar senha
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Email ou senha inválidos"
                    };
                }

                // Gerar token JWT
                var token = _jwtService.GenerateToken(user);
                var expiresAt = _jwtService.GetTokenExpiry();

                return new LoginResponseDTO
                {
                    Success = true,
                    Message = "Login realizado com sucesso",
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    ExpiresAt = expiresAt
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Erro interno do servidor"
                };
            }
        }

        public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO registerDto)
        {
            try
            {
                // Verificar se email já existe
                if (await _authRepository.EmailExistsAsync(registerDto.Email))
                {
                    return new RegisterResponseDTO
                    {
                        Success = false,
                        Message = "Email já está em uso"
                    };
                }

                // Hash da senha
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Criar usuário
                var user = new TbUser
                {
                    Name = registerDto.Name,
                    Email = registerDto.Email.ToLower(),
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createdUser = await _authRepository.CreateUserAsync(user);

                return new RegisterResponseDTO
                {
                    Success = true,
                    Message = "Usuário cadastrado com sucesso",
                    UserId = createdUser.Id,
                    Email = createdUser.Email,
                    Name = createdUser.Name
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponseDTO
                {
                    Success = false,
                    Message = "Erro interno do servidor"
                };
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                return await _authRepository.DeleteUserAsync(userId);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
