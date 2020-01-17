using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace clientapp
{
    class Program
    {
        private static IConfiguration _configuration;
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            _configuration = builder.Build();

            var rabbitMQConfigurations = new RabbitMQConfigurations();
            new ConfigureFromConfigurationOptions<RabbitMQConfigurations>(
                _configuration.GetSection("RabbitMQConfigurations"))
                    .Configure(rabbitMQConfigurations);

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQConfigurations.HostName,
                Port = rabbitMQConfigurations.Port,
                UserName = rabbitMQConfigurations.UserName,
                Password = rabbitMQConfigurations.Password
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "TesteRabbitMQ",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicConsume(queue: "TesteRabbitMQ",
                     autoAck: true,
                     consumer: consumer);

                Console.WriteLine("Aguardando mensagens para processamento");
                Console.WriteLine("Pressione uma tecla para encerrar...");
                Console.ReadKey();
            }
        }


        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine(Environment.NewLine +
                "[Nova mensagem recebida] " + message);
        }
    }
    
}
