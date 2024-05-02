namespace Challenge02.Domain.Models
{
    public class Dev
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Squad { get; set; }
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
