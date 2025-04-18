using Microsoft.EntityFrameworkCore;
using ProjectManagementApp.Model.Entity;

namespace ProjectManagementApp.Helpers.Context
{
    public class ProjectDbContext : DbContext
    {

        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {

        }

        public DbSet<mstProjecttbl> mstProjecttbl { get; set; }
        public DbSet<mst_User> mst_User { get; set; }
        public DbSet<mst_Role> mst_Role { get; set; }
    }
}
