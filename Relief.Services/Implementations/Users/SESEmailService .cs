using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Microsoft.Extensions.Configuration;
using Relief.ServiceAbstraction.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Users
{
    public class SESEmailService : IEmailService
    {
        private readonly IAmazonSimpleEmailServiceV2 _sesClient;
        private readonly string _fromEmail;

        public SESEmailService(IAmazonSimpleEmailServiceV2 sesClient, IConfiguration config)
        {
            _sesClient = sesClient;
            _fromEmail = config["AWS:SESFromEmail"]
            ?? throw new Exception("SES FromEmail not configured.");
        }
        public async Task SendVerificationCodeAsync(string toEmail, string code)
        {
            var request = new SendEmailRequest
            {
                FromEmailAddress = _fromEmail,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { toEmail }
                },
                Content = new EmailContent
                {
                    Simple = new Message
                    {
                        Subject = new Content { Data = "Your Verification Code" },
                        Body = new Body
                        {
                            Text = new Content
                            {
                                Data = $"Your verification code is: {code}\n\nThis code expires in 15 minutes."
                            }
                        }
                    }
                }
            };

            await _sesClient.SendEmailAsync(request);
        }
    }
}
