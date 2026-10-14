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
    internal class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.HasIndex(w => new { w.UserId, w.BookId }).IsUnique();

            builder.HasOne(w => w.Book)
                .WithMany()
                .HasForeignKey(w => w.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
