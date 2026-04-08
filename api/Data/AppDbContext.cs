using EmployeePayroll.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeePayroll.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
