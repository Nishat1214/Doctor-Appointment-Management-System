using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test1.Models;

namespace Test1.Data
{
    public class Test1Context : DbContext
    {
        public Test1Context (DbContextOptions<Test1Context> options)
            : base(options)
        {
        }

        public DbSet<Test1.Models.Admin> Admin { get; set; } = default!;
        public DbSet<Test1.Models.Appointment> Appointment { get; set; } = default!;
        public DbSet<Test1.Models.Doctor> Doctor { get; set; } = default!;
        public DbSet<Test1.Models.Patient> Patient { get; set; } = default!;
        public DbSet<Test1.Models.Speciality> Speciality { get; set; } = default!;
    }
}
