using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Product.API.EntityConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Entities.Product>
    {
        public void Configure(EntityTypeBuilder<Entities.Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
