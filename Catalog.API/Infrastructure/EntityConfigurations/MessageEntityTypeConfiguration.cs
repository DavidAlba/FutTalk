using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Infrastructure.EntityConfigurations
{
    public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message");

            builder.Property(m => m.Id)
            // Remove sequence (Incompatible with non-relational databases and other relational database providers (e.g. SqlLite)
            //.ForSqlServerUseSequenceHiLo("message_hilo")
            .ValueGeneratedNever()
            .IsRequired(true);

            builder.Property(m => m.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(m => m.Body)
                .IsRequired(false);
        }
    }
}
