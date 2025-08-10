using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using Application.DTOs;
using Domain.Interfaces;

namespace Application.Services
{
    public class EmailSenderQueueProducer : IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "email-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var message = new EmailMessageDto
            {
                ToEmail = toEmail,
                Subject = subject,
                Body = body
            };

            var bodyBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: "email-queue",
                                 basicProperties: properties,
                                 body: bodyBytes);

            return Task.CompletedTask;
        }
    }
}
