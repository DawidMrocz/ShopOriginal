using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.RabbitMessages.UserApp
{
    public class UserDeleted
    {
        public Guid Id { get; set; }
    }
}
