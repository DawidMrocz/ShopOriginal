using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeContracts.RabbitMessages.UserApp
{
    public class UserUpdated
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}
