using MediatR;
using Challenge02.Domain.Models;

namespace Challenge02.Domain.Commands
{
    public class AddDevCommand : IRequest<Dev>
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Squad { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
    }
}
