using DatabaseLibrary.Contexts;
using DatabaseLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Service
{
    public class CinemaUserService(CinemaUserDbContext context)
    {
        private readonly CinemaUserDbContext _context = context;
        public async Task AddUser(CinemaUser user)
        {
            await _context.CinemaUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<CinemaUser?> FindUserByLogin(string login)
        => await _context.CinemaUsers
        .Where(u => u.Login == login)
        .FirstOrDefaultAsync();

        public async Task<CinemaUser?> FindUserByLoginAndPasswordHash(string login, string passwordHash) 
            => await _context.CinemaUsers
                .Where(u => u.Login == login && u.PasswordHash == passwordHash)
                .FirstOrDefaultAsync();

        public async Task Add
    }
}
