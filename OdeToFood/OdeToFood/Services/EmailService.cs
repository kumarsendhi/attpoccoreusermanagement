using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;

namespace OdeToFood.Services
{
    public interface IEmailService
    {
        Task<Response> SendAsync(string to, string subject, string body);
    }
    public class EmailService : IEmailService
    {
        private string _apikey;

        public EmailService(IConfiguration configuration)
        {
            _apikey = configuration["SendGridKey"];
        }

        public async Task<Response> SendAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(_apikey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("kumar@kumar.com", "Testing Team"),
                Subject = subject,
                HtmlContent = body
            };
            msg.AddTo(new EmailAddress(to));
            return await client.SendEmailAsync(msg);
            
        }
    }
}
