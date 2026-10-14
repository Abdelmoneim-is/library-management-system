using LibraryDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Data.Configirations
{
    internal class AuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            builder.ToTable("Authors");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FullName)
                .HasMaxLength(150);

            builder.Property(a => a.Bio)
                .HasMaxLength(1000);

            builder.Property(a => a.Nationality)
                .HasMaxLength(100);

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.UpdatedAt)
                .IsRequired();

        }
    }
}
