using EmployeeApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Helper
{
    public class DataBaseContext :DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options):base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ParentDepartment> ParentDepartments { get; set; } = null!;
        public DbSet<ChildDepartment> ChildDepartments { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee", "dbo");
            modelBuilder.Entity<ParentDepartment>().ToTable("ParentDepartment");
            modelBuilder.Entity<ChildDepartment>().ToTable("ChildDepartment");
            modelBuilder.Entity<ParentDepartment>()
              .HasKey(p => p.DepartmentId);
            modelBuilder.Entity<ChildDepartment>()
                .HasOne(c => c.ParentDepartment)
                 .WithMany(p => p.ChildDepartments)
                .HasForeignKey(c => c.ParentDeptId)
                .HasPrincipalKey(p => p.DepartmentId);
        }
    }

}
