using System.Net;
using System.Text;
using System.Net.Mail;
using RabbitMQ.Client;
using System.Text.Json;
using Application.DTOs;
using RabbitMQ.Client.Events;

namespace Application.Worker
{
    public class EmailQueueConsumer
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "sistemyoneticisi11@gmail.com";
        private readonly string _smtpPassword = "twfjudhscgweieax";

        public void Start()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "email-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<EmailMessageDto>(Encoding.UTF8.GetString(body));

                if (message != null)
                {
                    await SendEmail(message);
                }

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "email-queue",
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine(" [*] Waiting for email messages. Press [enter] to exit.");
            Console.ReadLine();
        }

        private async Task SendEmail(EmailMessageDto email)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email.ToEmail);

            var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine($"📧 Email sent to {email.ToEmail}");
        }
    }
}
