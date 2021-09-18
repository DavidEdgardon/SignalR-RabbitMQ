using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PostSignalR.Models;
using RabbitMQ.Client;

namespace PostSignalR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendMsgController : ControllerBase
    {
        [HttpPost]
        public ActionResult<MessageDto> sendMessage([FromBody] MessageDto msg)
        {
            var xd = msg;

            try
            {
               var json = JsonConvert.SerializeObject(msg);
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    Port = 5672
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare("receive-msj-queue", false, false, false, null);
                        var body = Encoding.UTF8.GetBytes(json);
                        channel.BasicPublish(string.Empty, "receive-msj-queue", null, body);
                    }
                }
                return Ok($"Mensaje Enviado con exito!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
