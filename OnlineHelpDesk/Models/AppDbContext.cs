using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<Account> Account { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Discussion> Discussion { get; set; }
        public DbSet<Period> Period { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Ticket> Ticket { get; set; }

        public DbSet<Status> Status { get; set; }
        public DbSet<Photo> Photo { get; set; }


        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
            });

            model.Entity<Account>(entity =>
            {
                entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);

                entity.Property(e => e.FName)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);

                entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

                entity.HasOne(d => d.Role)
                .WithMany(p => p.Account)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");
            });
            model.Entity<Status>(entity =>
            {
                entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);

                entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
                entity.Property(e => e.Display).HasColumnName("Display");
            });

            model.Entity<Photo>(entity =>
            {
                entity.HasOne(d => d.Ticket)
                .WithMany(p => p.Photo)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Photo_Ticket");
            });

            model.Entity<Discussion>(entity =>
            {
                entity.Property(e => e.Content).HasColumnType("text");
                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.HasOne(d => d.Account)
                .WithMany(p => p.Discussion)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Discussion_Account");

                entity.HasOne(d => d.Ticket)
                .WithMany(p => p.Discussion)
                .HasForeignKey(d => d.TickerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Discussion_Ticket");
            });

            model.Entity<Ticket>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.HasOne(d => d.Category)
                .WithMany(p => p.Ticket)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Category");

                entity.HasOne(d => d.Period)
                .WithMany(p => p.Ticket)
                .HasForeignKey(d => d.PeriodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Period");

                entity.HasOne(d => d.User)
                .WithMany(p => p.TicketUser)
                .HasForeignKey(d=>d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Supporter)
                .WithMany(p => p.TicketSupporter)
                .HasForeignKey(d=>d.SupporterId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            });
        }
    }
}
