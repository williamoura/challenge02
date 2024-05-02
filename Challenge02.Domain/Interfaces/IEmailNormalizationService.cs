using Challenge02.Domain.Models;

namespace Challenge02.Domain.Interfaces
{
    public interface IEmailNormalizationService
    {
        bool TryNormalizeEmailDomain(Dev dev);
    }
}