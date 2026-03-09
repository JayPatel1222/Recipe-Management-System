using MailKit.Net.Smtp;
using MimeKit;


namespace EmailService
{
    public class EmailConfiguration
    {
        public string? From { get; set; }
        public string? smtpServer { get; set; }
        public int Port { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public interface IEmailSender
    {
        void SendEmail(EmailMessage message);
    }
    public class EmailSeneder : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSeneder(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(EmailMessage message)
        {
            var emailMesage = CreateEmailMessage(message);
            Send(emailMesage);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(string.Empty, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body =

                new TextPart(MimeKit.Text.TextFormat.Text)
                {
                    Text = message.Content
                };
            return emailMessage;
        }

        private void Send(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.smtpServer, _emailConfig.Port);
                    client.Send(message);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }

            }
        }
    }

    public class EmailMessage
    {
        public List<MailboxAddress>? To { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }

        public EmailMessage(IEnumerable<string> to, string? subject, string content)
        {
            //Populate the Collection of Mailbox Addresss using.....LINQ
            this.To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress(String.Empty, x)));

            this.Subject = subject;
            this.Content = content;
        }
    }
}
