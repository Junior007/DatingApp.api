using System.Collections.Generic;
using DatingApp.api.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.api.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options){}

        public DbSet<User> Users{get;set;}
        public DbSet<Value> Values{get;set;}
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
    //
}