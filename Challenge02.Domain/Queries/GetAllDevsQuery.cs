using MediatR;
using Challenge02.Domain.Models;

namespace Challenge02.Domain.Queries
{
    public class GetAllDevsQuery : IRequest<IEnumerable<Dev>>
    {
    }
}
