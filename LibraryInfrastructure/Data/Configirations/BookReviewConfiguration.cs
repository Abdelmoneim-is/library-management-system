using LibraryDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Data.Configirations
{
    public class BookReviewConfiguration : IEntityTypeConfiguration<BookReview >
    {
        public void Configure(EntityTypeBuilder<BookReview> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Comment)
                .HasMaxLength(1000);
            builder.HasIndex(r => new { r.BookId, r.UserId })
                .IsUnique();
            builder.HasOne(r => r.Book)
                .WithMany()
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
