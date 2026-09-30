using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.RabbitMessages.Favorite
{
    public class FavoriteCreated
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
    }
}
