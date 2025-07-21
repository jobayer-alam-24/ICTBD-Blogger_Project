using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogger.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Blogger.Areas.Administrator.Models;

namespace Blogger.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }
        public DbSet<Post> Posts {get; set;}
        public DbSet<Category> Categories {get; set;}
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Blogger.Areas.Administrator.Models.Person> Person { get; set; }

    }
}