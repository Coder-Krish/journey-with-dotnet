using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccess.Configurations;

public class EmployeeConfigurations:IEntityTypeConfiguration<Employees>
{
    public void Configure(EntityTypeBuilder<Employees> builder)
    {
        builder.HasKey(x => x.EmployeeId);
        builder.Property(t => t.FullName)
            .HasMaxLength(200)
            .IsRequired();
    }
}