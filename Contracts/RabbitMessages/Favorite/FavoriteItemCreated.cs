using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.RabbitMessages.Favorite
{
    public class FavoriteItemCreated
    {
        public int Id { get; set; } 
        public int FavoriteId { get; set; }
        public int ProductId { get; set; }
    }
}
