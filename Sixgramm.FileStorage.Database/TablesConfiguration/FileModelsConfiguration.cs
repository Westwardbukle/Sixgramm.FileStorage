using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sixgramm.FileStorage.Database.Models;

namespace Sixgramm.FileStorage.Database.TablesConfiguration;

public class FileModelsConfiguration: IEntityTypeConfiguration<FileModel>
{
    public void Configure(EntityTypeBuilder<FileModel> builder)
    {
        builder.Property(p => p.DateCreated).HasColumnType("timestamp without time zone").IsRequired();
        builder.Property(f => f.Name).IsRequired().HasMaxLength(50);
        builder.Property(f => f.UserId).IsRequired().HasMaxLength(50);
        builder.Property(f => f.Path).IsRequired().HasMaxLength(256);
        builder.Property(f => f.Length).IsRequired();
        builder.Property(f => f.Types).IsRequired().HasMaxLength(30);
        builder.Property(f => f.SourceId).IsRequired();
        builder.Property(f => f.FileSource).IsRequired();
    }
}