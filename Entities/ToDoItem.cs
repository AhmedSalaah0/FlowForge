namespace Entities
{
    public class ToDoItem
    {
        public Guid ItemId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Success { get; set; } = false;
        public Guid GroupId { get; set; }
    }
}
