using MediatR;

namespace Challenge02.Domain.Commands
{
    public class UpdateDevCommand : IRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Squad { get; set; }
        public string? Login { get; set; }
        public string? Email { get; set; }
    }
}
