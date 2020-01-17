using System;
using System.Text;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrdersController : ControllerBase
    {

        [HttpPost]
        public IActionResult Post([FromServices]RabbitMQConfigurations configurations,Order order)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                        HostName = configurations.HostName,
                        Port = configurations.Port,
                        UserName = configurations.UserName,
                        Password = configurations.Password
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "TesteRabbitMQ",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                        string message =
                            $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                            $"product: {order.ProductId}";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                            routingKey: "TesteRabbitMQ",
                                            basicProperties: null,
                                            body: body);
                }

                return StatusCode(200, "processing purchase order");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
    
