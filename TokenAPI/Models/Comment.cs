namespace TokenAPI.Models
{
    public class Comment
    {
        public Guid? Id { get; set; }
        public string Description { get; set; }
        public bool Moderate { get; set; } = false;
    }
}
