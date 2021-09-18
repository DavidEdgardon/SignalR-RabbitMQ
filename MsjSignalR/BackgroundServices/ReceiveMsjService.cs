using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using MsjSignalR.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MsjSignalR.BackgroundServices
{
    public class ReceiveMjsService : BackgroundService
    {
        private readonly IConnection _connection; //la conexión
        private readonly IModel _channel; // el canal
        private readonly EventingBasicConsumer _consumer; //consumidor

        private readonly IHubContext<ChatHub, IHubMsg> hubContext;
        public ReceiveMjsService(IHubContext<ChatHub, IHubMsg> hubContext)
        {
            this.hubContext = hubContext;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("receive-msj-queue", false, false, false, null);
            _consumer = new EventingBasicConsumer(_channel);
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Received += async (model, content) =>
            {
                var body = content.Body.ToArray();
                var result = Encoding.UTF8.GetString(body);
                await hubContext.Clients.All.MessageUpdated(result);
             
            };

            _channel.BasicConsume("receive-msj-queue", true, _consumer);
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
