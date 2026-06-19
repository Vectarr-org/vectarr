using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context
{
    public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : class, IBaseModel
    {
        public BaseEntityConfiguration()
        {

        }

        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.LastModified).IsRequired();
        }
    }
}
