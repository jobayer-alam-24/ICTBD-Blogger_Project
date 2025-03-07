using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogger.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }
        public DbSet<Post> Posts {get; set;}
        public DbSet<Category> Categories {get; set;}
        
    }
}