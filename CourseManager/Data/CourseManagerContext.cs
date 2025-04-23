using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CourseManager.Models;

namespace CourseManager.Data
{
    public class CourseManagerContext : DbContext
    {
        public CourseManagerContext (DbContextOptions<CourseManagerContext> options)
            : base(options)
        {
        }

        public DbSet<CourseManager.Models.Course> Course { get; set; } = default!;
        public DbSet<CourseManager.Models.User> User { get; set; } = default!;
        public DbSet<CourseManager.Models.Registration> Registration { get; set; } = default!;
    }
}
