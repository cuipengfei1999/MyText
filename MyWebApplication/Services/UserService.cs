using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApplication.Contexts;
using NuGet.Protocol;

namespace MyWebApplication.Services
{
    public class UserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context) {
            _context = context;
        }

        /// <summary>
        /// AddUsers
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public async Task<int> AddUsers(List<User> para)
        {
            await Console.Out.WriteLineAsync(para.ToJson());
            await _context.User.AddRangeAsync(para);
            var result = _context.SaveChanges();
            return result;
        }


        /// <summary>
        /// GetDbUser
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetDbUser()
        {
            return await _context.User.ToListAsync();
        }

        /// <summary>
        /// DeleteUsers
        /// </summary>
        /// <returns></returns>

        public async Task<int> DeleteUsers()
        {
            return await _context.User.ExecuteDeleteAsync();
        }
    }
}
