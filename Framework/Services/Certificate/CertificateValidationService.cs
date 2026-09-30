using Framework.Configuration;
using Microsoft.AspNetCore.Authentication.Certificate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Services.Certificate
{
    internal class CertificateValidationService : ICertificateValidationService
    {
        public bool ValidateCertificate(CertificateValidatedContext context)
        {
            if (FrameworkConfiguration.CertificateThumbprints.Any() && FrameworkConfiguration.CertificateThumbprints.Select(x => x.ToLower()).Contains(context.ClientCertificate.Thumbprint.ToLower()))
            {
                return true;
            }
            return false;
        }
    }
}
