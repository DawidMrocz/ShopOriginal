using Microsoft.AspNetCore.Authentication.Certificate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Services.Certificate
{
    public interface ICertificateValidationService
    {
        bool ValidateCertificate(CertificateValidatedContext context);
    }
}
