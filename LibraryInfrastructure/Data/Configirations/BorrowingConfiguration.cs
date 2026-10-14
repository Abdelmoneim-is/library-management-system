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
    public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
    {
        public void Configure(EntityTypeBuilder<Borrowing> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(b => b.FineAmount)
                   .HasPrecision(18, 2);
            builder.HasIndex(b => new { b.UserId, b.ReturnDate });
            builder.HasOne(b => b.Book)
                .WithMany()
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.FineAmount).HasPrecision(18, 2);
        }
    }
}
