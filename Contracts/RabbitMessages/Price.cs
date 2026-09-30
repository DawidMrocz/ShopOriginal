using Microsoft.EntityFrameworkCore;

namespace Contracts.RabbitMessages
{
    [Owned]
    public class Price
    {
        public decimal Value { get; set; }
        public string Currency { get; set; } = null!;
    }
}
