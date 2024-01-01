using EasyReading.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyReading.Infrastructure.Persistence.EntityTypeConfigurations
{
    internal class PageTypeConfiguration : IEntityTypeConfiguration<Page>
    {
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            builder.HasOne(p => p.Document).
                WithMany(p=>p.Pages).
                HasForeignKey(p=>p.DocumentId);
        }
    }
}
