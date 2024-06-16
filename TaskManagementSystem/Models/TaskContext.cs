public class TaskContext : DbContext
{
    public DbSet<Task> Tasks { get; set; }
}