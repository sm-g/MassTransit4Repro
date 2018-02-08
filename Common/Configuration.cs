using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Configuration
    {
        public static readonly string RabbitHost = "rabbitmq://localhost/mt4r";
        public static readonly string OperationARoutingKey = "A";
        public static readonly string QueueAName = "a_q";
        public static readonly string QueueUser1Name = "user1_q";
    }
}