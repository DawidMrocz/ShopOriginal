using ServiceBroker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBroker.Models
{
    public class MessageHandler
    {
        public string Type { get; set; }
        public IMessageHandler Handler { get; set; }
    }
}
