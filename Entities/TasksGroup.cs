namespace Entities
{
    public class TasksGroup
    {
        public Guid GroupId { get; set; }
        public string? groupTitle { get; set; }
        public string selectedColor { get; set; }
        public DateTime? createdAt { get; set; }
    }
}
