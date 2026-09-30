using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeContracts.RabbitMessages.UserApp
{
    public class UserDeleted
    {
        public Guid Id { get; set; }
    }
}
