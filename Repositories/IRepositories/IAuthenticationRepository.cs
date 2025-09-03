using WebApplication.Data.Entities;
using WebApplication.Models.Authentication;

namespace WebApplication.Repositories.IRepositories
{
    public interface IAuthenticationRepository
    {
        Task<TbUser?> GetUserByEmailAsync(string email);
        Task<TbUser?> GetUserByIdAsync(int id);
        Task<TbUser> CreateUserAsync(TbUser user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}
