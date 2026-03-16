using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces.Users
{
    public interface IEmailService
    {
        Task SendVerificationCodeAsync(string toEmail, string code);
    }
}
