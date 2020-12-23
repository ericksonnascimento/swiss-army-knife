using System;
using System.Text;
using RabbitMQ.Client;

namespace Pub
{
    class Program
    {
        private const string Hostname = "localhost";
        private const string Username = "admin";
        private const string Password = "admin";
        private const string VirtualHost = "Swiss";
        private const string QueueName = "queue-1";

        static void Main(string[] args)
        {
            Console.WriteLine("Type your message or 'exit' to escape");
            var message = Console.ReadLine();
            using (var connection = GetConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    QueueDeclare(channel);

                    while (message != "exit")
                    {
                        PublishMessage(channel, message);

                        message = Console.ReadLine();
                    }

                    channel.Close();
                }

                connection.Close();
            }
        }

        static IConnection GetConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = Hostname,
                UserName = Username,
                Password = Password,
                VirtualHost = VirtualHost
            };

            return factory.CreateConnection();
        }

        static void QueueDeclare(IModel channel)
        {
            channel.QueueDeclare(queue: QueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
        }

        static void PublishMessage(IModel channel, string message)
        {
            try
            {
                Console.WriteLine("-> Trying to publish your message...");

                var messageBytes = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                    routingKey: QueueName,
                                    basicProperties: null,
                                    body: messageBytes);

                Console.WriteLine($"-> Message '{message}' published.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"-> Error while publishing message: {ex.Message}");
            }
        }
    }
}
