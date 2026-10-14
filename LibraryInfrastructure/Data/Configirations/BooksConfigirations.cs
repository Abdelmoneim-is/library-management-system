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
    public class BooksConfigirations : IEntityTypeConfiguration<Books>
    {
        public void Configure(EntityTypeBuilder<Books> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .HasMaxLength(200);

            builder.Property(b => b.ISBN)
                .HasMaxLength(20);

            builder.HasIndex(b => b.ISBN)
                .IsUnique();

            builder.Property(b => b.Description)
                .HasMaxLength(1000);

            builder.Property(b => b.Publisher)
                .HasMaxLength(150);

            builder.Property(b => b.Language)
                .HasMaxLength(50);

            builder.Property(b => b.CoverImageURL)
                .HasMaxLength(500);

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(b => b.BookAuthor)
                .WithMany()
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.BookCategory)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.RowVersion)
                .IsRowVersion();
        }
    }
}
