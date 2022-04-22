using Note.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.DataAccessLayer.EF
{
    public class DatabaseContext:DbContext
    {
        public DbSet<NotUser> NotUsers { get; set; }
        public DbSet<Not> Nots { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Liked> Likes { get; set; }

        public DatabaseContext()
        {
            Database.SetInitializer(new MyInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //FluentAPI
            modelBuilder.Entity<Not>()
                .HasMany(x => x.Comments)
                .WithRequired(x => x.Nots)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Not>()
               .HasMany(x => x.Likes)
               .WithRequired(x => x.Nots)
               .WillCascadeOnDelete(true);


        }
    }
}
