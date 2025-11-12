using AuthLibrary.Contexts;
using AuthLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthLibrary.Service
{
    public class AuthService(CinemaUserDbContext context)
    {
        private readonly CinemaUserDbContext _context = context;

        #region task3
        private string HashPassword(string password)
            => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public async Task<bool> RegistrateUserAsync(string login, string password)
        {
            if (login.IsNullOrEmpty() || password.IsNullOrEmpty())
                return false;

            var selectedUser = await FindUserByLoginAsync(login);

            if (selectedUser is not null)
                return false;

            CinemaUser user = new()
            {
                Login = login,
                PasswordHash = HashPassword(password),
                Role = await GetRoleByNameAsync("Посетитель"),
            };

            try
            {
                await AddUserAsync(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CinemaUser?> AuthUserAsync(string login, string password)
        {
            var user = await FindUserByLoginAsync(login);
            if (user is null)
                return null;

            if (user.LockedUntil.HasValue)
                await UnlockUserAsync(user);

            if (user.LockedUntil.HasValue)
                return null;

            if (VerifyPassword(password, user.PasswordHash))
                return user;
           
            await IncreaseFailedLoginAttemptsAsync(user);
            return null;
        }

        public async Task<CinemaUserRole?> GetUserRoleByLoginAsync(string login)
        {
            var user = await _context.CinemaUsers
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == login);
            return user?.Role;
        }

        public async Task<IEnumerable<CinemaPrivilege>?> GetUserPrivilegesByLoginAsync(string login)
        {
            var role = await GetUserRoleByLoginAsync(login);
            return role?.Privileges.ToList();
        }

        public async Task<IEnumerable<CinemaPrivilege>?> GetCinemaPrivilegesByRole(CinemaUserRole role)
        {
            var selectedRole = await GetRoleByNameAsync(role.Name);
            return selectedRole?.Privileges.ToList();
        }

        #endregion

        private async Task AddUserAsync(CinemaUser user)
        {
            await _context.CinemaUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        private async Task<CinemaUser?> FindUserByLoginAsync(string login)
            => await _context.CinemaUsers
                .FirstOrDefaultAsync(u => u.Login == login);

        private bool VerifyPassword(string password, string passwordHash)
            => BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);

        private async Task IncreaseFailedLoginAttemptsAsync(CinemaUser user)
        {
            int attempts = 3;
            int minutes = 1;

            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= attempts)
            {
                user.FailedLoginAttempts = 0;
                user.LockedUntil = DateTime.Now.AddMinutes(minutes);
            }
            await _context.SaveChangesAsync();
        }

        private async Task UnlockUserAsync(CinemaUser user)
        {
            if (user.LockedUntil <= DateTime.Now)
            {
                user.LockedUntil = null;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<CinemaUserRole?> GetRoleByNameAsync(string name)
            => await _context.CinemaUserRoles
                .FirstOrDefaultAsync(r => r.Name == name);
    }
}
