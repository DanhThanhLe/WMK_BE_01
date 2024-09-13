//using Lucene.Net.Support;
//using RabbitMQ.Client;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WMK_BE_BusinessLogic.Service.Implement
//{
//    public class RabitMQProducer : IRabitMQProducer
//    {
//        public void SendProductMessage(IConnection connection, IModel channel, byte[] body)
//        {
//			try
//			{
//                var props = new HashMap<string, object>();
//                channel.QueueDeclareNoWait(queue: "checkorder",
//                          durable: true,
//                          exclusive: false,
//                          autoDelete: false,
//                          arguments: null);
//                channel.BasicPublish(exchange: string.Empty,
//                         routingKey: "checkorder",
//                         basicProperties: null,
//                         body: body);
//            }
//			catch (Exception)
//			{

//				throw;
//			}
//        }
//    }
//}
