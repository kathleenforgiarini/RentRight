using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentRight.Models;
using RentRight.Models.Enums;

namespace RentRight.Data
{
    public class RentRightContext : DbContext
    {
        public RentRightContext (DbContextOptions<RentRightContext> options)
            : base(options)
        {
        }

        public DbSet<RentRight.Models.User> User { get; set; } = default!;
        public DbSet<RentRight.Models.Property> Property { get; set; } = default!;
        public DbSet<RentRight.Models.Apartment> Apartment { get; set; } = default!;
        public DbSet<RentRight.Models.Message> Message { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Owner",
                    LastName = "Owner",
                    Email = "owner@owner.com",
                    Password = "1234",
                    Type = TypeUsers.Owner.ToString(),
                    IsActive = true,
                }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
