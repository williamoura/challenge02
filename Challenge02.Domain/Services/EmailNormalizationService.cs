using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;

namespace Challenge02.Domain.Services
{
    public class EmailNormalizationService : IEmailNormalizationService
    {
        public bool TryNormalizeEmailDomain(Dev dev)
        {
            if (dev.Email != null && !dev.Email.EndsWith("@challenge.com.br"))
            {
                dev.Email = TransformEmail(dev.Email);
                return true;
            }

            return false;
        }

        private static string TransformEmail(string email) => email.Split('@')[0] + "@challenge.com.br";
    }
}
