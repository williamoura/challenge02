using MediatR;
using Challenge02.Domain.Models;

namespace Challenge02.Domain.Queries
{
    public class GetDevByIdQuery : IRequest<Dev>
    {
        public int Id { get; set; }
    }
}
