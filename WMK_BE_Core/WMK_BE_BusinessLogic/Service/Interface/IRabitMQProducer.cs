using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
namespace WMK_BE_BusinessLogic.Service.Implement
{
    public interface IRabitMQProducer
    {
        public void SendProductMessage(IConnection connection, RabbitMQ.Client.IModel channel, byte[] body);
    }
}
