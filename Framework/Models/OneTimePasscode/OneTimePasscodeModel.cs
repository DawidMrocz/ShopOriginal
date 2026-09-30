using Framework.Models.Setting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Models.OneTimePasscode
{
    public class OneTimePasscodeModel<T> : BaseModel<int>
    {
        public T UserId { get; set; }
        public string Passcode { get; set; } = null!;
        public DateTime ExpireDate { get; set; }
        public Guid Guid { get; set; }
        public bool IsAuthTokenPersistent { get; set; }
    }

    public class OneTimePasscodeEntityTypeConfiguration<T> : BaseModelConfiguration<OneTimePasscodeModel<T>, int>
    {
        public override void Configure(EntityTypeBuilder<OneTimePasscodeModel<T>> modelBuilder)
        {
            base.Configure(modelBuilder);
        }
    }
}
