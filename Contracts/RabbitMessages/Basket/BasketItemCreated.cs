using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.RabbitMessages.Basket
{
    public class BasketItemCreated
    {
        public int ProductId { get; set; }
        public Guid UserId { get; set;}
        public int Quantity { get; set;}
        public int ProductQuantity { get; set;}
    }
}
