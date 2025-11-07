using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace UmaPay.Shared
{
    using Interface.Shared;
    public class MailService : IDisposable, IMailService
    {
        #region Private Fields
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;
        private bool _disposed;
        #endregion

        #region Constructor
        public MailService(
            IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _smtpClient = ConfigureSmtpClient();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Sends an email asynchronously
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content (HTML)</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task SendAsync(string to, string subject, string body)
        {
            try
            {
                ValidateEmailParameters(to, subject, body);
                using var message = CreateEmailMessage(to, subject, body);

                // Headers para autenticación
                message.Headers.Add("Return-Path", "grupouma.apps@grupouma.com");
                message.Headers.Add("Message-ID", $"<{Guid.NewGuid()}@grupouma.com>");
                //message.Headers.Add("X-SES-CONFIGURATION-SET", "ConfigurationSetName");

                await _smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new EmailServiceException("Failed to send email. See inner exception for details.", ex);
            }
        }
        #endregion

        #region Private Methods

        private SmtpClient ConfigureSmtpClient()
        {
            try
            {
                return new SmtpClient(_configuration["MailSettings:Host"])
                {
                    Port = GetConfigurationValue<int>("MailSettings:Port"),
                    Credentials = new System.Net.NetworkCredential(
                        _configuration["MailSettings:Username"],
                        _configuration["MailSettings:Password"]
                    ),
                    EnableSsl = GetConfigurationValue<bool>("MailSettings:EnableSsl"),
                    Timeout = GetConfigurationValue<int>("MailSettings:Timeout", 30000)
                };
            }
            catch (Exception ex)
            {
                throw new EmailServiceException(
                    "Failed to configure SMTP client. See inner exception for details.", ex);
            }
        }

        private T GetConfigurationValue<T>(string key, T defaultValue = default)
        {
            try
            {
                var value = _configuration[key];
                if (string.IsNullOrEmpty(value))
                {
                    return defaultValue;
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
        }

        private void ValidateEmailParameters(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email is required", nameof(to));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject is required", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body is required", nameof(body));

            if (!IsValidEmail(to))
                throw new ArgumentException("Invalid email address", nameof(to));
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private MailMessage CreateEmailMessage(string to, string subject, string body)
        {
            var fromAddress = _configuration["MailSettings:From"];
            return new MailMessage(fromAddress, to)
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = body,
                Priority = MailPriority.Normal
            };
        }
        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _smtpClient?.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }

    public class EmailServiceException : Exception
    {
        public EmailServiceException(string message) : base(message) { }
        public EmailServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }

}