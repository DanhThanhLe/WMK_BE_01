using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_BusinessLogic.Service.Implement
{
    //public class RabitMQConsumer : IHostedService
    //{
    //    private IModel channel = null;
    //    private IConnection connection = null;


        //private void Run()
        //{
        //    var factory = new ConnectionFactory()
        //    {
        //        HostName = "wemealkit.ddns.net",
        //        Port = 30008,
        //        VirtualHost = "/",
        //        UserName = "admin",
        //        Password = "admin"
        //    };
        //    factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(15);
        //    connection = factory.CreateConnection();
        //    channel = connection.CreateModel();
        //    channel.QueueDeclare(queue: "checkorder",
        //                        durable: true,
        //                        exclusive: false,
        //                        autoDelete: false,
        //                        arguments: null);
        //    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        //    Console.WriteLine(" [*] Waiting for messages.");
        //    var consumer = new EventingBasicConsumer(this.channel);
        //    consumer.Received += OnMessageRecieved;
        //    channel.BasicConsume(queue: "checkorder",
        //                        autoAck: false,
        //                        consumer: consumer);
        //}

        //public Task StartAsync(CancellationToken cancellationToken)
        //{
        //    Run();
        //    return Task.CompletedTask;
        //}

        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //    channel.Dispose();
        //    connection.Dispose();
        //    return Task.CompletedTask;
        //}


        //private async void OnMessageRecieved(object model, BasicDeliverEventArgs args)
        //{
        //    var body = args.Body.ToArray();
        //    var message = Encoding.UTF8.GetString(body);
        //    var data = JsonConvert.DeserializeObject<RabitMQSendData>(message);
        //    var orderId = data.OrderId;
        //    //call api to zalopay and set transaction status

         
        //    Console.WriteLine(" [x] Done");
        //    channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
        //}

    //}
}
