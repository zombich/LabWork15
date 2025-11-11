using DatabaseLibrary.Contexts;
using DatabaseLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthLibrary.Service
{
    public class AuthService(CinemaUserDbContext context)
    {
        private readonly CinemaUserDbContext _context = context;
        public async Task AddUserAsync(CinemaUser user)
        {
            await _context.CinemaUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<CinemaUser?> FindUserByLoginAsync(string login)
            => await _context.CinemaUsers
                .Where(u => u.Login == login)
                .FirstOrDefaultAsync();

        public async Task<CinemaUser?> FindUserByLoginAndPasswordHashAsync(string login, string passwordHash)
            => await _context.CinemaUsers
                .Where(u => u.Login == login && u.PasswordHash == passwordHash)
                .FirstOrDefaultAsync();

        public async Task IncreaseUserFailedLoginAttemptsByLoginAsync(string login)
        {
            var user = await FindUserByLoginAsync(login);
            if (user is null)
                return;

            if (user.FailedLoginAttempts >= 3)
            {
                user.FailedLoginAttempts = 0;
                user.LockedUntil = DateTime.Now.AddMinutes(1);
            }
            else
                user.FailedLoginAttempts++;

            await _context.SaveChangesAsync();
        }

        public async Task UnlockUserByLoginAsync(string login)
        {
            var user = await FindUserByLoginAsync(login);
            if (user is null)
                return;

            if (user.LockedUntil <= DateTime.Now)
            {
                user.LockedUntil = null;
                await _context.SaveChangesAsync();
            }
        }

        private string HashPassword(string password)
            => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public async Task<bool> RegistrateUserAsync(string login, string password)
        {
            CinemaUserRole role = new()
            {
                RoleId = 3,
                Name = "Пользователь"
            };

            CinemaUser user = new()
            {
                Login = login,
                PasswordHash = HashPassword(password),
                Role = role,
            };

            var selectedUser = await FindUserByLoginAsync(login);
            if (selectedUser is null)
            {
                await AddUserAsync(user);
                return true;
            }

            return false;
        }

        public async Task<CinemaUser> AuthUserAsync(string login, string password)
        {
            var passwordHash = HashPassword(password);
            var selectedUser = await FindUserByLoginAndPasswordHashAsync(login, passwordHash);

            if (selectedUser is null)
            {
                await IncreaseUserFailedLoginAttemptsByLoginAsync(login);
                return null;
            }

            return selectedUser;
        }
        
        public CinemaUserRole GetUserRoleByLogin(string login)
            => _context.CinemaUserRoles.Include(r => r.CinemaUsers).
    }
}
