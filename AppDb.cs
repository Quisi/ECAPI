using EnergyScanApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Linq;
namespace EnergyScanApi
{
    public class AppDb : DbContext
    {
        public MySqlConnection Connection { get; }
        public AppDb(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            if (configuration["Database:insertCorePolicys"].ToString().Equals("true"))
            {
                //Database.Migrate();
                if (Policies.FirstOrDefault(p => p.Name.Equals("read")) == null)
                {
                    Policies.Add(new Policy() { Name = "read", Id = Guid.NewGuid().ToString(), CreationDate = DateTime.Now, ChangedLast = DateTime.Now });
                }
                if (Policies.FirstOrDefault(p => p.Name.Equals("create")) == null)
                {
                    Policies.Add(new Policy() { Name = "create", Id = Guid.NewGuid().ToString(), CreationDate = DateTime.Now, ChangedLast = DateTime.Now });
                }
                if (Policies.FirstOrDefault(p => p.Name.Equals("update")) == null)
                {
                    Policies.Add(new Policy() { Name = "update", Id = Guid.NewGuid().ToString(), CreationDate = DateTime.Now, ChangedLast = DateTime.Now });
                }
                if (Policies.FirstOrDefault(p => p.Name.Equals("delete")) == null)
                {
                    Policies.Add(new Policy() { Name = "delete", Id = Guid.NewGuid().ToString(), CreationDate = DateTime.Now, ChangedLast = DateTime.Now });
                }
                if (Policies.FirstOrDefault(p => p.Name.Equals("usermanager")) == null)
                {
                    Policies.Add(new Policy() { Name = "usermanager", Id = Guid.NewGuid().ToString(), CreationDate = DateTime.Now, ChangedLast = DateTime.Now });
                }
                if (Policies.FirstOrDefault(p => p.Name.Equals("requestmanager")) == null)
                {
                    Policies.Add(new Policy() { Name = "requestmanager", Id = Guid.NewGuid().ToString(), CreationDate = DateTime.Now, ChangedLast = DateTime.Now });
                }
                if (Policies.FirstOrDefault(p => p.Name.Equals("policymanager")) == null)
                {
                    Policies.Add(new Policy() { Name = "policymanager", Id = Guid.NewGuid().ToString(), CreationDate = DateTime.Now, ChangedLast = DateTime.Now });
                }
                if (Policies.FirstOrDefault(p => p.Name.Equals("groupmanager")) == null)
                {
                    Policies.Add(new Policy() { Name = "groupmanager", Id = Guid.NewGuid().ToString(), CreationDate = DateTime.Now, ChangedLast = DateTime.Now });
                }
                SaveChanges();
                SettingsHelpers.AddOrUpdateAppSetting("Database:insertCorePolicys", "false");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<BarcodeCan> BarcodeCans { get; set; }
        public DbSet<Can> Cans { get; set; }
        public DbSet<ChangeRequest> ChangeRequests { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ImageCan> ImageCans { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagCan> TagCans { get; set; }
        public DbSet<Taste> Tastes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPolicy> UserPolicies { get; set; }

    }
}
