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

        public DbSet<RentRight.Models.User> Users { get; set; } = default!;
        public DbSet<RentRight.Models.Property> Properties { get; set; } = default!;
        public DbSet<RentRight.Models.Apartment> Apartments { get; set; } = default!;
        public DbSet<RentRight.Models.Message> Messages { get; set; } = default!;
        public DbSet<RentRight.Models.Rental> Rentals { get; set; } = default!;
        public DbSet<RentRight.Models.ManagerAvailability> ManagerAvailabilities { get; set; } = default!;
        public DbSet<RentRight.Models.Appointments> Appointments { get; set; } = default!;

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

            modelBuilder.Entity<Apartment>()
                .HasOne(a => a.Property)
                .WithMany()
                .HasForeignKey(a => a.PropertyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Appointments>()
                .HasOne(ap => ap.Apartment)
                .WithMany()
                .HasForeignKey(ap => ap.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Appointments>()
                .HasOne(ap => ap.Manager)
                .WithMany()
                .HasForeignKey(ap => ap.ManagerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Appointments>()
                .HasOne(ap => ap.Tenant)
                .WithMany()
                .HasForeignKey(ap => ap.TenantId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<ManagerAvailability>()
                .HasOne(ma => ma.Manager)
                .WithMany()
                .HasForeignKey(ma => ma.ManagerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Apartment)
                .WithMany()
                .HasForeignKey(m => m.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Property)
                .WithMany()
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Tenant)
                .WithMany()
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
