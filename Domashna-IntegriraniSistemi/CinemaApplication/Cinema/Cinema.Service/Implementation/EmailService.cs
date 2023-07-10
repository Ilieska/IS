using Cinema.Domain;
using Cinema.Domain.DomainModels;
using Cinema.Service.Interface;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(EmailSettings settings)
        {
            _settings = settings;
        }

        public async Task SendEmailAsync(List<EmailMessage> allMails)
         {

         }
           
    }
}
