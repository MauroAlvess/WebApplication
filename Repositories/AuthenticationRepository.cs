using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Data.Entities;
using WebApplication.Repositories.IRepositories;

namespace WebApplication.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly AppDbContext _context;

        public AuthenticationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TbUser?> GetUserByEmailAsync(string email)
        {
            return await _context.TbUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
        }

        public async Task<TbUser?> GetUserByIdAsync(int id)
        {
            return await _context.TbUsers
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<TbUser> CreateUserAsync(TbUser user)
        {
            _context.TbUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.TbUsers.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            if (user == null)
                return false;

            // Soft delete
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.TbUsers
                .AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
        }
    }
}
