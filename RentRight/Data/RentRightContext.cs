using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentRight.Models;

namespace RentRight.Data
{
    public class RentRightContext : DbContext
    {
        public RentRightContext (DbContextOptions<RentRightContext> options)
            : base(options)
        {
        }

        public DbSet<RentRight.Models.User> User { get; set; } = default!;
    }
}
