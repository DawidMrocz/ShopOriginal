using Jobs.Infrastructure.Attributes;
using System.ComponentModel;

namespace Jobs.Enums
{
    public enum JobTypeEnum
    {
        /// <summary>
        /// Nieznany JobType Enum
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Usuwa stare basket itemy
        /// </summary>
        [Description("Usuówa stare itemy")]
        [ImplementationJob("DeleteBasketItemsJob")]
        SendInvoiceJob = 1,
    }
}
