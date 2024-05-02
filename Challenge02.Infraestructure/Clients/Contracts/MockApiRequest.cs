namespace Challenge02.Infraestructure.Clients.Contracts
{
    public class MockApiRequest
    {
    }

    public class CreateDevsRequest : MockApiRequest
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Squad { get; set; }
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class UpdateDevsRequest : MockApiRequest
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Squad { get; set; }
        public string? Login { get; set; }
        public string? Email { get; set; }
    }

    public class GetDevsRequest : MockApiRequest
    {
        public GetDevsRequest(int? id = null)
        {
            Id = id;
        }

        public int? Id { get; set; }
    }
}
