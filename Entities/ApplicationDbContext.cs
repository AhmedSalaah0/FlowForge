using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public virtual DbSet<TasksGroup> groups { get; set; }
        public virtual DbSet<ToDoItem> tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TasksGroup>().ToTable("groupTasks").HasKey("GroupId");
            modelBuilder.Entity<ToDoItem>().ToTable("ToDoItems").HasKey("ItemId");
            modelBuilder.Entity<ToDoItem>()
                .HasOne<TasksGroup>()
                .WithMany()
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
