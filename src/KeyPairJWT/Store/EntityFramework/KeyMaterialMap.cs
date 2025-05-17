using KeyPairJWT.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyPairJWT.Store.EntityFramework;

public class KeyMaterialMap : IEntityTypeConfiguration<KeyMaterial>
{
    public void Configure(EntityTypeBuilder<KeyMaterial> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Parameters)
            .HasMaxLength(8000)
            .IsRequired();
    }
}
