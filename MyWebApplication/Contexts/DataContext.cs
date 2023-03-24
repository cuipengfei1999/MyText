using Microsoft.EntityFrameworkCore;

namespace MyWebApplication.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public DataContext(DbContextOptions<DataContext> options)
            :base(options)
        {
        
        }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<User> User { get; set; }
    }
}
