using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectTask> Tasks { get; set; }
        public virtual DbSet<ProjectMember> ProjectMembers { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProjectMember>()
            .HasKey(ug => new { ug.MemberId, ug.ProjectId });

            modelBuilder.Entity<ProjectMember>()
                .HasOne(ug => ug.Member)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.MemberId);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(ug => ug.Project)
                .WithMany(g => g.ProjectMembers)
                .HasForeignKey(ug => ug.ProjectId);

            modelBuilder.Entity<Project>().ToTable("Projects").HasKey("ProjectId");
            modelBuilder.Entity<ProjectTask>().ToTable("ProjectTasks").HasKey("TaskId");

            modelBuilder.Entity<ProjectTask>()
                .HasOne<Project>()
                .WithMany()
                .HasForeignKey(t=> t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<ProjectTask>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.MemberId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Project>()
            .HasOne(g => g.CreatedBy)
            .WithMany()
            .HasForeignKey(g => g.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NotificationId);

            modelBuilder.Entity<Notification>()
                .HasOne(n=> n.Receiver)
                .WithMany()
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Sender)
                .WithMany()
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Project)
                .WithMany()
                .HasForeignKey(n => n.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
