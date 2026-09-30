using Microsoft.EntityFrameworkCore;

namespace BackOfficeContracts.RabbitMessages
{
    [Owned]
    public class Price
    {
        public decimal Value { get; set; }
        public string Currency { get; set; } = null!;
    }
}
