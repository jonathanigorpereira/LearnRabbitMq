using Api.Model;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Api.Controllers
{
    [Route("api/messages")]
    public class MassagesController : ControllerBase
    {
        private const string QUEUE_NAME = "message";
        private readonly ConnectionFactory _factory;
        public MassagesController()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
        }

        [HttpPost]
        public IActionResult Post([FromBody] SendMessage send)
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Declarar a fila
                    channel.QueueDeclare(
                        queue: QUEUE_NAME,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var jsonMessage = JsonSerializer.Serialize(send);
                    var bytes = Encoding.UTF8.GetBytes(jsonMessage);

                    channel.BasicPublish(
                        exchange:"",
                        routingKey:QUEUE_NAME,
                        basicProperties: null,
                        body: bytes
                        );
                }
            }
            return Accepted();
        }
    }
}
