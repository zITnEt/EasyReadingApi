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
    internal class DocumentTypeConfiguration: IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(p => p.User).
                WithMany(p => p.Documents).
                HasForeignKey(p=>p.UserId);
        }
    }
}
